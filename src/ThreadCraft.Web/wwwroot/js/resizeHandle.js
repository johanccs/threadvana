// Drag-to-resize handle between the theory column and the workspace column.
// Uses a CSS custom property (--theory-frac) so the grid-template-columns rule
// stays in the stylesheet and the collapsed state keeps working.

(function () {
    const KEY = 'threadcraft-theory-ratio';
    let handle = null, grid = null, theory = null;
    let dragging = false, startX = 0, startFrac = 0.4;

    window.ThreadCraft = window.ThreadCraft || {};
    window.ThreadCraft.initResizeHandle = function (handleId, gridSelector) {
        handle = document.getElementById(handleId);
        grid = document.querySelector(gridSelector);
        if (!handle || !grid) return;
        theory = grid.querySelector('.theory-column');

        const saved = sessionStorage.getItem(KEY);
        if (saved !== null) startFrac = clamp(parseFloat(saved));
        applyFrac(startFrac);

        handle.addEventListener('mousedown', onMouseDown);
    };

    function onMouseDown(e) {
        if (!grid || !theory) return;
        dragging = true;
        startX = e.clientX;
        // Measure the ACTUAL pixel split instead of parsing gridTemplateColumns.
        const tw = theory.getBoundingClientRect().width;
        const gw = grid.getBoundingClientRect().width;
        startFrac = clamp(tw / gw);
        handle.classList.add('dragging');
        document.addEventListener('mousemove', onMouseMove);
        document.addEventListener('mouseup', onMouseUp);
        e.preventDefault();
    }

    function onMouseMove(e) {
        if (!dragging || !grid) return;
        const gw = grid.getBoundingClientRect().width;
        const newFrac = clamp(startFrac + (e.clientX - startX) / gw);
        applyFrac(newFrac);
    }

    function onMouseUp() {
        dragging = false;
        document.removeEventListener('mousemove', onMouseMove);
        document.removeEventListener('mouseup', onMouseUp);
        if (handle) handle.classList.remove('dragging');
    }

    function applyFrac(frac) {
        grid.style.setProperty('--theory-frac', frac.toFixed(3));
        sessionStorage.setItem(KEY, frac.toFixed(3));
    }

    function clamp(v) { return Math.min(0.65, Math.max(0.2, v || 0.4)); }
})();

