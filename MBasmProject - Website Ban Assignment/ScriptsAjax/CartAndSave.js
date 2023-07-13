$(document).ready(function () {
    $('#btn-save').on('click', function () {
        var asm_id = $(this).closest('button').data('id');

            $.ajax({
                url: "/User/home/SavedFiles",
                method: "POST",
                data: { asmId: asm_id },
                success: function (rs) {
                    if (rs.success) {
                        var messageContainer = $('#messageCart');
                        messageContainer.attr('style', 'margin: auto; width: fit-content; color: #007bff;').text('Success!');
                        messageContainer.fadeOut(3000);
                    }
                    else {
                        if (rs.exist) {
                            var messageContainer = $('#messageCart');
                            messageContainer.attr('style', 'margin: auto; width: fit-content; color: #b60000;').text('You saved this!');
                            messageContainer.fadeOut(3000);
                        }
                        else {
                            var messageContainer = $('#messageCart');
                            messageContainer.attr('style', 'margin: auto; width: fit-content; color: #b60000;').text('Please Sign in!');
                            messageContainer.fadeOut(3000);
                        }
                    }
                }
            });
        });


        $('#btn-addCart').on('click', function () {
            var asm_id = $(this).closest('button').data('id');

            $.ajax({
                url: "/User/Cart/AddCart",
                method: "POST",
                data: { asmId: asm_id },
                success: function (rs) {
                    if (rs.success) {
                        var messageContainer = $('#messageCart');
                        messageContainer.attr('style', 'margin: auto; width: fit-content; color: #007bff;').text('Success!');
                        messageContainer.fadeOut(3000);
                    }
                    else {
                        if (rs.purchase) {
                            var messageContainer = $('#messageCart');
                            messageContainer.attr('style', 'margin: auto; width: fit-content; color: #b60000;').text('You purchased this!');
                            messageContainer.fadeOut(4000);
                        }
                        else if (rs.exist) {
                            var messageContainer = $('#messageCart');
                            messageContainer.attr('style', 'margin: auto; width: fit-content; color: #b60000;').text('Existed in cart!');
                            messageContainer.fadeOut(3000);
                        }
                        else {
                            var messageContainer = $('#messageCart');
                            messageContainer.attr('style', 'margin: auto; width: fit-content; color: #b60000;').text('Please Sign in!');
                            messageContainer.fadeOut(3000);
                        }
                    }
                }
            });
        });

});