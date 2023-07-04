$(document).ready(function () {
    $('#SearchingForm').on('submit', function (e) {
        e.preventDefault();

        var formData = new FormData(this);

        $.ajax({
            url: "/Subject",
            method: "GET",
            data: formData,
            success: function (rs) {
                if (rs.success) {
                    alert("Create new assignment successfully!");
                    window.location.reload();
                }
                else {
                    alert("Fail to create new assignment!");
                    window.location.reload();
                }
            }
        });

    });
});