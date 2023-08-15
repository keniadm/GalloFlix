function showLoading() {
    const loader = document.querySelector('.loader');
    if (loader != null) {
        loader.classList.remove('d-none');
    }
};

function hideLoading() {
    const loader = document.querySelector('.loader');
    if (loader != null) {
        loader.classList.add('d-none');
    }
};

$(document).ready(function () {
    hideLoading();
});

document.addEventListener('DOMContentLoaded', function() {
    var form = document.querySelector('form');
    console.log(form);
    if (form != undefined){ 
        form.addEventListener('submit', function() {
            if ($(form).valid() === true) 
                showLoading();
        })
    }
});

(() => {
    'use strict';
    if (document.querySelector('#sidebarToggler') != null) {
        document.querySelector('#sidebarToggler').addEventListener('click', () => {
            document.querySelector('#sidebar').classList.toggle('d-none')
        })
    }
})()