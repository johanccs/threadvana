// Tiny helpers for the "Ask the coach" panel.
window.ThreadCraftChat = {
    // Keeps the transcript pinned to the newest message.
    scrollToEnd: function (el) {
        if (el) {
            el.scrollTop = el.scrollHeight;
        }
    }
};
