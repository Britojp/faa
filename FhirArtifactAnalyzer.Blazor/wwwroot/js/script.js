    window.blazorFileDrop = {
        preventDragDefaults: function (elementId) {
            const el = document.getElementById(elementId);
    if (!el) return;

            ['dragenter', 'dragover', 'dragleave', 'drop'].forEach(eventName => {
        el.addEventListener(eventName, (e) => {
            e.preventDefault();
            e.stopPropagation();
        }, false);
            });
        }
    };
