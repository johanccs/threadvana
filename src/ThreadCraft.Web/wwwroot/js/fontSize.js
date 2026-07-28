// Per-panel text-size persistence (localStorage), one independent key per panel.
window.ThreadCraft = window.ThreadCraft || {};
window.ThreadCraft.fontSize = {
    get: function (key, fallback) {
        var raw = localStorage.getItem('threadcraft-fontsize-' + key);
        var parsed = raw !== null ? parseInt(raw, 10) : NaN;
        return isNaN(parsed) ? fallback : parsed;
    },
    set: function (key, value) {
        localStorage.setItem('threadcraft-fontsize-' + key, value);
    }
};
