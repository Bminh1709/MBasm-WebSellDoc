$(document).ready(function () {

    // VALIDATE
    //function formValidCat() {
    //    f = document.frmAddCat;

    //    if (f.Name.value == "") {
    //        alert("Name can't be empty");
    //        f.Name.focus();
    //        return false;
    //    }
    //    // alert("You have filled out all the information.");
    //    return true;
    //}

    // ADD ASSIGNMENT
    $('#AssignmentForm').on('submit', function (e) {
        e.preventDefault(); // Prevent the form from submitting normally

        var formData = new FormData(this);

        $.ajax({
            url: '/Admin/Subject/AddAsm',
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
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
    // GET ASM ON FORM
    $('body').on('click', '.btnEditAsm', function () {
        var tmpId = $(this).data("id");
        $.ajax({
            url: '/Admin/Subject/GetAsmById',
            type: 'GET',
            data: { id: tmpId },
            success: function (rs) {
                if (rs.data != null && rs.categoryName != null) {
                    console.log(rs.data);
                    $('#InputAsmTitle').val(rs.data.Title);
                    $('#InputAsmCat').val(rs.categoryName); // Assuming categoryName is included in the JSON response
                    $('#InputAsmDes').val(rs.data.Description);
                    $('#InputAsmGrade').val(rs.data.Grade);
                    $('#InputAsmPrice').val(rs.data.Price);
                    $('#InputAsmHot').prop('checked', rs.data.Hot);
                }
            },
            error: function (xhr, status, error) {
                // Display error message
                console.log(xhr);
                console.log(status);
                console.log(error);
                alert('Error: ' + error);
            }
        });
    });

    // AFTER GET CAT => EDIT
    $('#CategoryFormUpdate').on('submit', function (e) {
        // Hide params on link
        e.preventDefault(); // Prevent the form from submitting normally

        var tmpCategory = {
            Id: $("#InputCatId").val(),
            Name: $("#InputCatName").val(),
            Description: $("#InputCatDescription").val()
        };
        $.ajax({
            url: '/Admin/Subject/UpdateCategory',
            type: 'POST',
            data: {
                category: tmpCategory
            },
            success: function (rs) {
                if (rs.success) {
                    window.location.reload();
                    alert("Update category successfully!");
                }
                else {
                    window.location.reload();
                    alert("Fail to update category!");
                }
            }
        });
    });

    //Delete Asm
    $('body').on('click', '.btnDeleteAsm', function () {
        var tmpId = $(this).data("id");
        // -- Jquery --
        var conf = confirm("Are you sure to delete this assignment?");
        // Ajax
        if (conf == true) {
            $.ajax({
                url: '/Admin/Subject/DeleteAsm',
                type: 'Post',
                data: { id: tmpId },
                success: function (rs) {
                    if (rs.success == true) {
                        $('#trow_' + tmpId).remove();
                    }
                }
            });
        }
    });






});