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

        // Reset the form
        $('#AssignmentFormUpdate')[0].reset();
        $('#ImageContainer').empty();

        $('#InputAsmFileDocxUpdate').hide();
        $('#InputAsmFileDocx').show();
        $('#InputAsmFilePdfUpdate').hide();
        $('#InputAsmFilePdf').show();
        $('#InputAsmFilePptxUpdate').hide();
        $('#InputAsmFilePptx').show();


        var tmpId = $(this).data("id");
        $.ajax({
            url: '/Admin/Subject/GetAsmById',
            type: 'GET',
            data: { id: tmpId },
            success: function (rs) {
                if (rs.data != null) {
                    $('#InputAsmId').val(rs.data.Id);
                    $('#InputAsmTitle').val(rs.data.Title);
                    // Get option of server-side code
                    $('#InputAsmCat option').filter(function () {
                        return $(this).text().trim() === rs.categoryName.trim();
                    }).prop('selected', true);
                    $('#InputAsmDes').val(rs.data.Description);
                    $('#InputAsmGrade').val(rs.data.Grade);
                    $('#InputAsmPrice').val(rs.data.Price);
                    $('#InputAsmHot').prop('checked', rs.data.Hot);

                    // Handle the link to split the GUID part => get the link on device
                    if (rs.data.FileDocx != null) {
                        let fileDocx = rs.data.FileDocx.split('_');
                        if (fileDocx.length > 1) {
                            var part1 = fileDocx.slice(0, 1)[0];
                            var part2 = fileDocx.slice(1).join('_');
                            $('#InputAsmFileDocx').val(part2);
                        } else {
                            $('#InputAsmFileDocx').val(rs.data.rs.data.FileDocx);
                        }
                    }
                    if (rs.data.FilePdf != null) {
                        let filePdf = rs.data.FilePdf.split('_');
                        if (filePdf.length > 1) {
                            var part1 = filePdf.slice(0, 1)[0];
                            var part2 = filePdf.slice(1).join('_');
                            $('#InputAsmFilePdf').val(part2);
                        } else {
                            $('#InputAsmFilePdf').val(rs.data.FilePdf);
                        }
                    }
                    if (rs.data.FilePptx != null) {
                        let filePptx = rs.data.FilePptx.split('_');
                        if (filePptx.length > 1) {
                            var part1 = filePptx.slice(0, 1)[0];
                            var part2 = filePptx.slice(1).join('_');
                            $('#InputAsmFilePptx').val(part2);
                        } else {
                            $('#InputAsmPptx').val(rs.data.FilePptx);
                        }
                    }
                    if (rs.data.Images != null && rs.data.Images.length !== 0) {
                        var imagesData = rs.data.Images;
                        var images = imagesData.split(',');

                        for (var i = 0; i < images.length; i++) {
                            var imgSrc = '/assets/img/Asm/' + images[i];

                            var divContainer = $('<div>').css('position', 'relative');
                            var iconDelete = $('<i>').addClass('fa-solid fa-trash-can').css('font-size', '25px');
                            var imgLink = $('<button>').css({
                                'position': 'absolute',
                                'right': '-10px',
                                'color': '#b73636',
                                'background': 'none',
                                'border': 'none'
                            }).attr('type', 'button').attr('id', 'btnDeleteImg')/*.attr('data-imgid', i)*/;
                            var imgElement = $('<img>').css({
                                'height': '170px',
                                'width': '130px',
                                'object-fit': 'cover',
                                'margin': '10px auto',
                                'border': 'var(--bs-border-width) solid #DFE5EF'
                            }).attr('src', imgSrc);

                            imgLink.append(iconDelete);
                            divContainer.append(imgLink);
                            divContainer.append(imgElement);
                            $('#ImageContainer').append(divContainer);
                        }
                    }

                    else {
                        var imgElement = $('<img>').attr('style', 'height: 150px; width: 100px; object-fit: cover; margin: 10px auto; border: var(--bs-border-width) solid #DFE5EF;').attr('src', "/assets/img/Asm/No-Image.svg.png");
                        $('#ImageContainer').append(imgElement);
                    }
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

    $('#btnUploadFileDocx').click(function (event) {
        //event.stopPropagation(); // Prevent the default form submission behavior
        event.preventDefault();
        // $('#InputAsmFileDocx').attr('type', 'file');
        $('#InputAsmFileDocx').hide();
        $('#InputAsmFileDocxUpdate').show().click();
    });
    $('#btnUploadFilePdf').click(function (event) {
        event.preventDefault();
        $('#InputAsmFilePdf').hide();
        $('#InputAsmFilePdfUpdate').show().click();
    });
    $('#btnUploadFilePptx').click(function (event) {
        event.preventDefault();
        $('#InputAsmFilePptx').hide();
        $('#InputAsmFilePptxUpdate').show().click();
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