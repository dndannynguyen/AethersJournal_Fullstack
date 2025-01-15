'use strict';

const dropdownIcon = document.getElementById("date-dropdown-icon");
const dropdownDiv = document.getElementById("date-list");

// dropdown menu drops down when clicked or dedrops if its already dropped
dropdownIcon.addEventListener("click", () => {
    if (dropdownDiv.style.opacity == 0) {
        dropdownDiv.style.zIndex = 99;
        dropdownDiv.style.opacity = 1;
        return;
    }
    dropdownDiv.style.opacity = 0;
    dropdownDiv.style.zIndex = -1;
});