$(document).ready(function () {
    $(document).on('click', '.addbasket', function (e) {
        e.preventDefault();

        //let url = $(this).attr('href');

        fetch($(this).attr('href'))
            .then(res => res.text())
            .then(data => { $('.header-cart').html(data) });
    })

    $(document).on('click', '.deletebasket', function (e) {
        e.preventDefault();

        //let url = $(this).attr('href');

        fetch($(this).attr('href'))
            .then(res => res.text())
            .then(data => { $('.header-cart').html(data) });
    })

    $(document).on('keyup', '.searchinput', function (e) {
        e.preventDefault();
        console.log($(this).val());
        console.log($(this).data("url"))
        let url = $(this).data("url") + '?search=' + $(this).val();
        console.log(url);

        if ($(this).val().trim().length > 2) {

            fetch(url)
                .then(res => res.text())
                .then(data => {
                    console.log(data);
                    $('.searchlist').html(data);
                })

            //Old Partial
            //fetch(url)
            //    .then(res => res.json())
            //    .then(data => {

            //        let listitemts = "";
            //        console.log(data)
            //        for (var i = 0; i < data.length; i++) {
            //            let listitem = `<li class="list-group-item">
            //                                    <img style="width:50px;" src="assets/images/product/${data[i].mainImage}" alt="Alternate Text" />
            //                                    ${data[i].title} 
            //                                </li>`

            //            listitemts += listitem;
            //        }

            //        $('.searchlist').html(listitemts)
            //    });
        }
        else {
            $('.searchlist').html("");
        }

        
    })

    $(document).on('click', '.productModal', function (e) {
        e.preventDefault();

        fetch($(this).attr('href'))
            .then(res => res.text())
            .then(data => {
                $('.modal-body').html(data);

                $('.quick-view-image').slick({
                    slidesToShow: 1,
                    slidesToScroll: 1,
                    arrows: false,
                    dots: false,
                    fade: true,
                    asNavFor: '.quick-view-thumb',
                    speed: 400,
                });

                $('.quick-view-thumb').slick({
                    slidesToShow: 4,
                    slidesToScroll: 1,
                    asNavFor: '.quick-view-image',
                    dots: false,
                    arrows: false,
                    focusOnSelect: true,
                    speed: 400,
                });
            })
    })

    $(document).on('click', '.subCount', function (e) {
        e.preventDefault();

        let count = $(this).next().val();

        if (count > 1) {
            count--;
            $(this).next().val(count)

            let url = $(this).attr('href') + '?count=' + count;
            console.log(url)
            fetch(url)
                .then(res => res.text())
                .then(data => {
                    $('.basketcontainer').html(data);
                    fetch('/basket/getbasket')
                        .then(res => res.text())
                        .then(data => { $('.header-cart').html(data) });
                })
        }
    })

    $(document).on('click', '.addCount', function (e) {
        e.preventDefault();

        let count = $(this).prev().val();

        if (count < 1) {
            count = 1
        }

        count++;
        $(this).prev().val(count)

        let url = $(this).attr('href') + '?count=' + count;
        console.log(url)
        fetch(url)
            .then(res => res.text())
            .then(data => {
                $('.basketcontainer').html(data);
                fetch('/basket/getbasket')
                    .then(res => res.text())
                    .then(data => { $('.header-cart').html(data) });
            })
    })

    $(document).on('click', '.deletefromcart', function (e) {
        e.preventDefault();

        fetch($(this).attr('href'))
            .then(res => res.text())
            .then(data => {
                $('.basketcontainer').html(data);
                fetch('/basket/getbasket')
                    .then(res => res.text())
                    .then(data => { $('.header-cart').html(data) });
            })
    })

    let pageindex = parseInt($('.pageindex').val());

    let page = pageindex;

    $(document).on('click', '.loadmore', function (e) {
        e.preventDefault();
        let productCount = parseInt($('.productCount').val());

        page++;
        
        let url = $(this).attr('href') + '?page=' + page;

        if (page <= productCount) {
            fetch(url)
                .then(res => res.text())
                .then(data => {
                    $('.productcontainer').append(data);
                })
        }

        if (page == productCount) {
            $(this).remove();
        }
    })

    console.log(window.location.pathname.split('/')[1]);
    let path = window.location.pathname.split('/')[1];
    let links = $('.header-horizontal-menu .menu-content li');
    console.log(links)

    for (var i = 0; i < links.length; i++) {
        console.log(links[i].children[0].getAttribute('href').split('/')[1])
        let linkpath = links[i].children[0].getAttribute('href').split('/')[1];
        if (path.trim().toLocaleLowerCase() == linkpath.trim().toLocaleLowerCase()) {
            links[i].classList.add('active');
        } else {
            links[i].classList.remove('active');
        }
    }

    $(document).on('show.bs.collapse','.accordian-body', function () {
        $(this).closest("table")
            .find(".collapse.in")
            .not(this)
        //.collapse('toggle')
    })
})