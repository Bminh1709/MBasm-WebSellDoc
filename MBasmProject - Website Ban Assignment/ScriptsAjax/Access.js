$(document).ready(function () {
    // VALIDATE
    function formValidUser() {
        // using name of form - not id or class
        f = document.frmUser;

        var email_pattern = /^[a-zA-Z]\w*(\w+)*\@\w+(\.\w{2,3})$/;

        if (f.Fullname.value == "") {
            alert("Your name is required");
            f.Fullname.focus();
            return false;
        }
        if (email_pattern.test(f.Gmail.value) == false) {
            alert("Invalid email address");
            f.Gmail.focus();
            return false;
        }
        if (f.Password.value.length < 8) {
            alert("Password must be at least 8 characters");
            f.Password.focus();
            return false;
        }
        return true;
    }


    // Initialize a flag variable
    var isRequestPending = false;

    $('#signupFormUser').on('submit', function (e) {
        e.preventDefault();

        // Check if a request is already pending
        if (isRequestPending) {
            return; // Exit early if a request is still pending
        }

        var formData = new FormData(this);

        if (formValidUser()) {
            isRequestPending = true; // Set the flag to indicate a request is pending

            $.ajax({
                url: '/Access/Signup',
                type: 'POST',
                data: formData,
                processData: false,
                contentType: false,
                success: function (rs) {
                    if (rs.success) {
                        $('#messageSignUp').attr('style', 'color: #345971').text('Sign up successfully!');
                        setTimeout(function () {
                            window.location.reload();
                        }, 2000);
                    } else {
                        var messageContainer = $('#messageSignUp');
                        messageContainer.attr('style', 'color: #ce3b2d').text('Account has been existed!');
                        messageContainer.fadeOut(3000);
                    }

                    isRequestPending = false; // Reset the flag after the request is complete
                },
                complete: function () {
                    isRequestPending = false; // Reset the flag in case of any errors or aborts
                }
            });
        }
    });


    $('#signinFormUser').on('submit', function (e) {
        e.preventDefault();

        var formData = new FormData(this);

        $.ajax({
            url: '/Access/Signin',
            method: 'Post',
            data: formData,
            processData: false,
            contentType: false,
            success: function (rs) {
                if (rs.success) {
                    $('#messageSignIn').attr('style', 'color: #345971').text('Sign in successfully!');
                    setTimeout(function () {
                        window.location = "/";  
                    }, 1000);
                }
                else {
                    var messageContainer = $('#messageSignIn');
                    messageContainer.attr('style', 'color: #ce3b2d').text('Uncorrect Gmail or Password!');
                    messageContainer.fadeOut(3000);
                }
            }
        });
    });

});