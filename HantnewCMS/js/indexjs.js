layui.use(['jquery', 'layer', 'element'], function () {
    window.jQuery = window.$ = layui.jquery;
    window.layer = layui.layer;
    window.element = layui.element();

    $('.panel .tools .iconpx-chevron-down').click(function () {
        var el = $(this).parents(".panel").children(".panel-body");
        if ($(this).hasClass("iconpx-chevron-down")) {
            $(this).removeClass("iconpx-chevron-down").addClass("iconpx-chevron-up");
            el.slideUp(200);
        } else {
            $(this).removeClass("iconpx-chevron-up").addClass("iconpx-chevron-down");
            el.slideDown(200);
        }
    })

});

