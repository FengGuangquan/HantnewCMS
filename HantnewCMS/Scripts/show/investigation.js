function mywjdc() {
    var mywjdc = document.getElementById('mywjdc');
    var wjdctable1 = document.getElementById('wjdctable1');
    var wjdctable2 = document.getElementById('wjdctable2');
    if (wjdctable1.style.display != 'block') {
        wjdctable1.style.display = 'block';
        wjdctable2.style.display = 'none';
        mywjdc.innerText = "我的问卷调查";
    } else {
        wjdctable1.style.display = 'none';
        wjdctable2.style.display = 'block';
        mywjdc.innerText = "返回";
    }
}