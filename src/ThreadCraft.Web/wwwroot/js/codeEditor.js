// Monaco editor wrapper for ThreadCraft Academy.
// Loaded from CDN; Blazor talks to it through the functions on window.threadCraftEditor.
window.threadCraftEditor = (function () {
    var CDN_BASE = "https://cdnjs.cloudflare.com/ajax/libs/monaco-editor/0.45.0/min/vs";
    var editors = {}; // elementId -> { editor, dotNetRef, suppressChange, debounceTimer }
    var monacoPromise = null;

    // Monaco web workers cannot be loaded cross-origin directly; this proxy
    // bootstraps the base worker from the CDN inside a data: URL.
    window.MonacoEnvironment = {
        getWorkerUrl: function () {
            var proxy =
                "self.MonacoEnvironment = { baseUrl: 'https://cdnjs.cloudflare.com/ajax/libs/monaco-editor/0.45.0/min/' };" +
                "importScripts('" + CDN_BASE + "/base/worker/workerMain.js');";
            return "data:text/javascript;charset=utf-8," + encodeURIComponent(proxy);
        }
    };

    function loadMonaco() {
        if (monacoPromise) {
            return monacoPromise;
        }

        monacoPromise = new Promise(function (resolve, reject) {
            if (window.monaco) {
                resolve(window.monaco);
                return;
            }

            var script = document.createElement("script");
            script.src = CDN_BASE + "/loader.min.js";
            script.onload = function () {
                window.require.config({ paths: { vs: CDN_BASE } });
                window.require(["vs/editor/editor.main"], function () {
                    resolve(window.monaco);
                });
            };
            script.onerror = function () {
                monacoPromise = null; // allow a retry on the next attempt
                reject(new Error("Could not load the code editor from the CDN."));
            };
            document.head.appendChild(script);
        });

        return monacoPromise;
    }

    function toMonacoSeverity(severity) {
        switch ((severity || "").toLowerCase()) {
            case "error": return monaco.MarkerSeverity.Error;
            case "warning": return monaco.MarkerSeverity.Warning;
            default: return monaco.MarkerSeverity.Info;
        }
    }

    return {
        // Creates an editor on the element and reports edits back to .NET
        // (debounced ~300ms so we don't round-trip on every keystroke).
        createAsync: function (dotNetRef, elementId, initialValue, readOnly) {
            return loadMonaco().then(function () {
                var host = document.getElementById(elementId);
                if (!host) {
                    throw new Error("Editor host element not found: " + elementId);
                }

                var editor = monaco.editor.create(host, {
                    value: initialValue || "",
                    language: "csharp",
                    theme: "vs-dark",
                    readOnly: !!readOnly,
                    automaticLayout: true,
                    minimap: { enabled: false },
                    fontSize: 13,
                    lineNumbers: "on",
                    scrollBeyondLastLine: false,
                    renderWhitespace: "none",
                    padding: { top: 10 },
                    scrollbar: { verticalScrollbarSize: 10, horizontalScrollbarSize: 10 }
                });

                var entry = { editor: editor, dotNetRef: dotNetRef, suppressChange: false, debounceTimer: null };
                editors[elementId] = entry;

                editor.onDidChangeModelContent(function () {
                    if (entry.suppressChange) {
                        return;
                    }
                    if (entry.debounceTimer) {
                        clearTimeout(entry.debounceTimer);
                    }
                    entry.debounceTimer = setTimeout(function () {
                        entry.dotNetRef.invokeMethodAsync("OnEditorValueChanged", editor.getValue());
                    }, 300);
                });

                return true;
            });
        },

        // Replaces the editor content (used when the parent resets the code).
        setValue: function (elementId, value) {
            var entry = editors[elementId];
            if (!entry) {
                return;
            }
            if (entry.editor.getValue() === value) {
                return;
            }
            entry.suppressChange = true;
            entry.editor.setValue(value);
            entry.suppressChange = false;
        },

        // Paints squiggles: markers is [{ line, column, message, severity }].
        setMarkers: function (elementId, markers) {
            var entry = editors[elementId];
            if (!entry) {
                return;
            }
            var model = entry.editor.getModel();
            if (!model) {
                return;
            }

            var lineCount = model.getLineCount();
            var monacoMarkers = (markers || []).map(function (m) {
                var line = Math.min(Math.max(1, m.line || 1), lineCount);
                return {
                    severity: toMonacoSeverity(m.severity),
                    message: m.message || "",
                    startLineNumber: line,
                    startColumn: 1,
                    endLineNumber: line,
                    endColumn: model.getLineMaxColumn(line)
                };
            });

            monaco.editor.setModelMarkers(model, "threadcraft", monacoMarkers);
        },

        // Moves the cursor to a location (used when the learner clicks a compile issue).
        setPosition: function (elementId, line, column) {
            var entry = editors[elementId];
            if (!entry) {
                return;
            }
            var model = entry.editor.getModel();
            var lineCount = model ? model.getLineCount() : 1;
            var targetLine = Math.min(Math.max(1, line || 1), lineCount);
            var targetColumn = Math.max(1, column || 1);
            entry.editor.setPosition({ lineNumber: targetLine, column: targetColumn });
            entry.editor.revealLineInCenterIfOutsideViewport(targetLine);
            entry.editor.focus();
        },

        dispose: function (elementId) {
            var entry = editors[elementId];
            if (!entry) {
                return;
            }
            if (entry.debounceTimer) {
                clearTimeout(entry.debounceTimer);
            }
            var model = entry.editor.getModel();
            entry.editor.dispose();
            if (model) {
                model.dispose();
            }
            delete editors[elementId];
        }
    };
})();
