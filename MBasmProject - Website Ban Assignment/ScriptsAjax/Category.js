﻿$(document).ready(function () {

    // VALIDATE
    function formValidCat() {
        f = document.frmAddCat;

        if (f.Name.value == "") {
            alert("Name can't be empty");
            f.Name.focus();
            return false;
        }
        // alert("You have filled out all the information.");
        return true;
    }

    // ADD CATEGORY
    $('#CategoryForm').on('submit', function (e) {
        e.preventDefault(); // Prevent the form from submitting normally

        var formData = new FormData(this);

        if (formValidCat()) {
            $.ajax({
                url: '/Admin/Subject/AddCat',
                type: 'POST',
                data: formData,
                // prevent jQuery from automatically processing the data => Can send file
                processData: false,
                contentType: false,
                success: function (rs) {
                    if (rs.success) {
                        alert("Create new category successfully!");
                        window.location.reload();
                    }
                    else {
                        alert("Fail to create new category!");
                        window.location.reload();
                    }
                }
            });
        }
    });
    // GET CAT ON FORM
    $('body').on('click', '.btnEditCat', function () {
        var tmpId = $(this).data("id");
        $.ajax({
            url: '/Admin/Subject/GetCatById',
            type: 'Get',
            data: { id: tmpId },
            success: function (rs) {
                if (rs.data != null) {
                    $('#InputCatId').val(rs.data.Id);
                    $('#InputCatName').val(rs.data.Name);
                    $('#InputCatDescription').val(rs.data.Description);
                }
            }
        });
    });
    // AFTER GET CAT => EDIT
    //$('#CategoryFormUpdate').on('submit', function (e) {
    //    // Hide params on link
    //    e.preventDefault(); // Prevent the form from submitting normally

    //    var tmpCategory = {
    //        Id: $("#InputCatId").val(),
    //        Name: $("#InputCatName").val(),
    //        Description: $("#InputCatDescription").val()
    //    };
    //    $.ajax({
    //        url: '/Admin/Subject/UpdateCategory',
    //        type: 'POST',
    //        data: {
    //            category: tmpCategory
    //        },
    //        success: function (rs) {
    //            if (rs.success) {
    //                window.location.reload();
    //                alert("Update category successfully!");
    //            }
    //            else {
    //                window.location.reload();
    //                alert("Fail to update category!");
    //            }
    //        }
    //    });
    //});
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


     //Delete Cat
    $('body').on('click', '.btnDeleteCat', function () {
        var tmpId = $(this).data("id");
        // -- Jquery --
        var conf = confirm("Are you sure to delete this Category?");
        // Ajax
        if (conf == true) {
            $.ajax({
                url: '/Admin/Subject/DeleteCat',
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