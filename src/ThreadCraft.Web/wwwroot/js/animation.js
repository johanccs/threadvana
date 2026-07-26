// Step-by-step animation driver for .anim-diagram blocks.
// Called from Blazor OnAfterRenderAsync so it re-runs after DOM replacement.
window.ThreadCraftAnim = {
    _intervals: [],

    initAll: function () {
        // Clear old intervals from previous renders.
        this._intervals.forEach(function (id) { clearInterval(id); });
        this._intervals = [];

        document.querySelectorAll('.anim-diagram').forEach(function (el) {
            // Reset init flag (Blazor may have cleared it, or this is a new render).
            el.removeAttribute('data-anim-init');
            if (el.dataset.animInit) return;
            el.dataset.animInit = '1';

            var codeCol = el.querySelector('.anim-code-col');
            var stateCol = el.querySelector('.anim-state-col');
            var narration = el.querySelector('.anim-narration');
            var dots = el.querySelector('.anim-dots');
            if (!codeCol) return;

            var lines = codeCol.querySelectorAll('.anim-line');
            var total = lines.length;
            if (total === 0) return;

            var stateBoxes = stateCol ? stateCol.querySelectorAll('.anim-thread-box') : [];
            var narrs = (el.getAttribute('data-narr') || 'Running on Thread A|Thread A still executing|Hits await — Thread A RELEASED to pool|Task completes — Thread B picks up|Method finishes — zero blocking').split('|');

            var current = 0;
            var self = this;
            var interval = null;

            if (dots) {
                dots.innerHTML = '';
                for (var i = 0; i < total; i++) {
                    var dot = document.createElement('span');
                    dot.className = 'anim-dot' + (i === 0 ? ' active' : '');
                    (function(idx) { dot.addEventListener('click', function () { goTo(idx); }); })(i);
                    dots.appendChild(dot);
                }
            }

            function render(step) {
                lines.forEach(function (line, i) {
                    line.classList.remove('highlight', 'dimmed');
                    if (i < step) line.classList.add('dimmed');
                    if (i === step) line.classList.add('highlight');
                });
                if (stateBoxes.length >= 2) {
                    stateBoxes.forEach(function (b) { b.className = 'anim-thread-box'; });
                    if (step < 3) stateBoxes[0].className = 'anim-thread-box running';
                    else if (step === 3) { stateBoxes[0].className = 'anim-thread-box released'; stateBoxes[1].className = 'anim-thread-box resumed'; }
                    else stateBoxes[1].className = 'anim-thread-box running';
                }
                if (narration) narration.innerHTML = '<span class="anim-narr-active">' + (narrs[step] || '') + '</span>';
                if (dots) dots.querySelectorAll('.anim-dot').forEach(function (d, i) { d.classList.toggle('active', i === step); });
            }

            function goTo(i) { current = Math.max(0, Math.min(i, total - 1)); render(current); resetTimer(); }
            function next() { current = (current + 1) % total; render(current); }
            function resetTimer() { if (interval) clearInterval(interval); interval = setInterval(next, 2500); self._intervals.push(interval); }

            render(0);
            resetTimer();
        });
    }
};