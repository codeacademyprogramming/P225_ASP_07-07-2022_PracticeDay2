$(document).ready(function () {
    $(document).on('click', '.deleteandrestorebtn', function (e) {
        e.preventDefault();
        Swal.fire({
            title: 'Are you sure?',
            text: "You won't be able to revert this!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes, delete it!'
        }).then((result) => {
            if (result.isConfirmed) {
                fetch($(this).attr('href'))
                    .then(res => res.text())
                    .then(data => {
                        $('.datacontainer').html(data)
                    })
                
                Swal.fire(
                    'Deleted!',
                    'Your file has been deleted.',
                    'success'
                )
            }
        })
    })

    $(document).on('click', '#IsMain', function ()
    {
        if ($(this).is(':checked')) {
            $('.imageInput').removeClass('d-none');
            $('.parentInput').addClass('d-none');
        } else {
            $('.imageInput').addClass('d-none');
            $('.parentInput').removeClass('d-none');
        }
    })

    $(document).on('click', '.productimageremovebtn', function (e) {
        e.preventDefault();

        fetch($(this).attr('href'))
            .then(res => res.text())
            .then(data => {
                $('.productimagescontainer').html(data)
            })
    })

    if ($('#IsMain').is(':checked')) {
        $('.imageInput').removeClass('d-none');
        $('.parentInput').addClass('d-none');
    } else {
        $('.imageInput').addClass('d-none');
        $('.parentInput').removeClass('d-none');
    }

    toastr.options = {
        "closeButton": false,
        "debug": false,
        "newestOnTop": false,
        "progressBar": true,
        "positionClass": "toast-top-right",
        "preventDuplicates": false,
        "onclick": null,
        "showDuration": "300",
        "hideDuration": "1000",
        "timeOut": "5000",
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    };
    //console.log($('#successInput').val().length)
    //console.log($('#successInput').val())
    if ($('#successInput').val().length)
    {
        console.log($('#successInput').val().length)
        console.log($('#successInput').val())
        toastr["success"]($('#successInput').val());
    }

    
    /*Command: toastr["success"]("I do not think that means what you think it means.")*/

    
})