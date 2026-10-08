'use strict';

$(document).ready(function () {

    function showFormError(form, message) {
        let box = $(form).find('.ajax-error-box');

        if (box.length === 0) {
            box = $('<div class="ajax-error-box"></div>')
                .css({
                    color: '#dc3545',
                    fontSize: '13px',
                    marginTop: '8px'
                });

            $(form).append(box);
        }

        box.text(message);
    }

    function clearFormError(form) {
        $(form).find('.ajax-error-box').text('');
    }

    function toggleBusy(form, busy) {
        let btn = $(form).find('[type="submit"]');

        if (busy) {
            btn.prop('disabled', true);
            btn.data('original', btn.html());
            btn.html('Please wait...');
        } else {
            btn.prop('disabled', false);

            if (btn.data('original')) {
                btn.html(btn.data('original'));
            }
        }
    }

    /* =========================
       ADD / EDIT FORM
    ========================= */

    $('form[data-ajax-form="true"]').on('submit', function (e) {
        e.preventDefault();

        let form = this;
        clearFormError(form);

        let url = $(form).data('url') || $(form).attr('action');
        let formData = new FormData(form);

        toggleBusy(form, true);

        $.ajax({
            url: url,
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,

            success: function (res) {
                toggleBusy(form, false);

                if (res && res.success) {

                    Swal.fire({
                        icon: 'success',
                        title: 'Success',
                        text: res.message || 'Saved successfully!',
                        timer: 1500,
                        showConfirmButton: false
                    }).then(() => {
                        location.reload();
                    });

                } else {
                    showFormError(form, res.message || 'Something went wrong.');
                }
            },

            error: function () {
                toggleBusy(form, false);

                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: 'Server error occurred'
                });
            }
        });
    });

    /* =========================
       DELETE (SweetAlert Confirm)
    ========================= */

    $(document).on('click', '[data-ajax-delete="true"]', function (e) {
        e.preventDefault();

        let btn = $(this);
        let url = btn.data('url');

        if (!url) return;

        Swal.fire({
            title: 'Are you sure?',
            text: "This record will be permanently deleted!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#d33',
            cancelButtonColor: '#3085d6',
            confirmButtonText: 'Yes, delete it!'
        }).then((result) => {

            if (!result.isConfirmed) return;

            btn.addClass('disabled');

            $.ajax({
                url: url,
                type: 'POST',

                success: function (res) {

                    if (res && res.success) {

                        Swal.fire({
                            icon: 'success',
                            title: 'Deleted!',
                            text: res.message || 'Record deleted successfully',
                            timer: 1500,
                            showConfirmButton: false
                        }).then(() => {
                            location.reload();
                        });

                    } else {
                        btn.removeClass('disabled');

                        Swal.fire({
                            icon: 'error',
                            title: 'Failed',
                            text: res.message || 'Could not delete record'
                        });
                    }
                },

                error: function () {
                    btn.removeClass('disabled');

                    Swal.fire({
                        icon: 'error',
                        title: 'Error',
                        text: 'Server error occurred'
                    });
                }
            });

        });
    });

    /* =========================
       EDIT MODAL FILL
    ========================= */

    $('.js-edit-branch').on('click', function () {

        $('#edit-branch-id').val($(this).data('id'));
        $('#edit-branch-name').val($(this).data('name'));
        $('#edit-branch-city').val($(this).data('city'));
        $('#edit-branch-email').val($(this).data('email'));
        $('#edit-branch-address').val($(this).data('address') || '');
        $('#edit-branch-contact').val($(this).data('contact') || '');
        $('#edit-branch-status').val($(this).data('status'));
    });

});