function myarticle() {
    var myarticle = document.getElementById('myarticle');
    var wztable1 = document.getElementById('wztable1');
    var wztable2 = document.getElementById('wztable2');
    if (wztable1.style.display != 'block') { 
        wztable1.style.display = 'block'; 
        wztable2.style.display = 'none';
        myarticle.innerText = "我的文章";
    } else {
        wztable1.style.display = 'none';
        wztable2.style.display = 'block';
        myarticle.innerText = "返回";
    }
}