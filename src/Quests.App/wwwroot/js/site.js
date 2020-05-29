// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your Javascript code.
$(function () {
    $('[role=textbox][name]').each(function () {
        window[this.name + "_simplemde"] = new SimpleMDE({
            element: this,
            forceSync: true,
            tabSize: 4
        });
    });
});
