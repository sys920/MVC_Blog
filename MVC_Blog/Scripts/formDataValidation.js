$(function () {
    $("#register-form").validate({
        rules: {
            searchKeyword: "required"
            
        },
        messages: {
            searchKeyword: "Please, Enter search keyword"
        },

        submitHandler: function (form) {
            form.submit();
        }
    });

});