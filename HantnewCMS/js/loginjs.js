$(function () {
    $(".layui-canvs").jParticle({
        background: "#141414",
        color: "#E6E6E6"
    });
    //登录链接测试，使用时可删除
    $(".submit_btn").click(function () {
        location.href="index.html";
    });
});