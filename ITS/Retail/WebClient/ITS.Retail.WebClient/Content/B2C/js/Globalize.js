

Globalize.addCultureInfo("el", {
    messages: {
        "addedToWishList": "Το είδος έχει προστεθεί στην Λίστα Επιθυμιών"
    }
});

Globalize.addCultureInfo("en", {
    messages: {
        "addedToWishList": "The item has been added to wishlist"
    }
});

function GetMessage(name) {
    return Globalize.localize(name, $.cookie("_culture"));
}