function myvote() {
    var myvote = document.getElementById('myvote');
    var tptable1 = document.getElementById('tptable1');
    var tptable2 = document.getElementById('tptable2');
    if (tptable1.style.display != 'block') {
        tptable1.style.display = 'block';
        tptable2.style.display = 'none';
        myvote.innerText = "我的投票";
    } else {
        tptable1.style.display = 'none';
        tptable2.style.display = 'block';
        myvote.innerText = "返回";
    }
}