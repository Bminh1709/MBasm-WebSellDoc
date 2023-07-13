$(document).ready(function () {
    $('.btn-deleteCart').on('click', function () {
        var cartID = $(this).data('id');

        $.ajax({
            url: "/User/Cart/DeleteCart",
            method: "POST",
            data: { id: cartID },
            success: function (rs) {
                if (rs.success) {
                    $('#trow_' + cartID).remove();
                    $('#totalPriceInput').text(rs.totalPrice);
                }
            }
        });
    });
    $('#btn-payment').on('click', function () {
        // Show loader icon
        showLoader();

        var tmpNote = $('textarea#InputNote').val();
        $.ajax({
            url: "/User/Cart/Payment",
            method: "POST",
            data: { note: tmpNote },
            success: function (rs) {
                if (rs.success) {
                    setTimeout(function () {
                        window.location.href = '/User/Cart/Payment';
                    }, 3000);
                }
                else {
                    alert("Please, select the assignment you want!");
                    window.location.reload();
                }
            }
        });

    });

    function showLoader() {
        // Display the loader icon
        $('#btn-payment').html('<i class="fa fa-spinner fa-spin"></i> Loading...');
    }

});