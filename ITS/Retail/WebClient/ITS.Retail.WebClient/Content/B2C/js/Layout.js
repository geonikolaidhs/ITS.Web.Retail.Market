var path = home + "B2C/";
var fullnameProfileForm = "";
var activeCategory = "";
var element = "";
var Layout = (function () {

    return {
        Init: function () {

            $('header span#menu').click(function () {
                $('nav#alternative_menu').slideToggle();
            });


            $('header .login-container .avatar').hover(function () {
                $('header .login-container #profile').show();
                $('header .login-container .avatar').addClass('background-color');
            });

            $('header .login-container .login').hover(function () {
                $('header .login-container #login').show();
                $('header .login-container .login').addClass('background-color');
            });

            $('header .cart a').hover(function () {
                $('header .cart a').addClass('background-color');
            });

            $('header .login-container').mouseleave(function () {
                $('header .profile-list').hide();
                $('header .login-container .avatar').removeClass('background-color');
                $('header .login-container .login').removeClass('background-color');
                $('header .cart a').removeClass('background-color');
            });


            $('.cart').click(function () {
                if ($("#menu-right_multilevelpushmenu .levelHolderClass").hasClass("multilevelpushmenu_inactive")) {
                    $('#menu-right').multilevelpushmenu('expand');
                }
                else {
                    $('#menu-right').multilevelpushmenu('collapse');
                    $('.cart').blur();
                }
            });



            $('.category').click(function () {
                if ($("#menu_multilevelpushmenu .levelHolderClass").hasClass("multilevelpushmenu_inactive")) {
                    $('#menu').multilevelpushmenu('expand');
                }
                else {
                    $('#menu').multilevelpushmenu('collapse');
                }
            });

            $('.cart-count').text(ShoppingCartItemsCount);

            Layout.PrepareCategoriesMenu();
            ShoppingCart();
            Products.ProductListing();
            $("#search-form-button").click(function (e) {
                $("#search-form-mobile").collapse('toggle');
            });



            $('.mobile-dropdown-container').on('show.bs.dropdown', function () {
                if ($(document).width() < 768) {
                    $("#search-form-mobile").collapse('hide');
                }
            });


            $("#wrapper").click(function () {

                $('#menu-right').multilevelpushmenu('collapse');
                $('.cart').blur();
                $('#menu').multilevelpushmenu('collapse');

                $('#menu #menu_multilevelpushmenu div').click(function (event) {
                    event.stopPropagation();
                });

                $('#menu-right .levelHolderClass.rtl').click(function (event) {
                    event.stopPropagation();
                });

            });

            $("#selectLanguage").find("li a").on({
                click: function () {
                    chooseLanguage($(this).data("lang"));
                }
            });

            function chooseLanguage(locale) {
                if ($.cookie("_culture") !== locale && $.cookie("_culture") !== null) {
                    $.cookie("_culture", locale, { expires: 365, path: '/' });
                    window.location.reload();
                }
            }

        },        
        ExpandShoppingCart: function () {
            $('#menu-right').multilevelpushmenu('expand');
        },
        Scrollbar: function (s, e) {
            //$('.DocumentDetails').customScrollbar();
        },
        ItemCategoriesTreeView_OnNodeClick: function (s, e) {
            var id = s.GetSelectedNode().name;
            var newLink = path + "Products?CategoryID=" + id;
            window.location.href = newLink;
        },
        setJSNotification: function (message) {
            if (msg != message) {
                msg = message;
                var n = noty({
                    text: message,
                    theme: 'bootstrap',
                    layout: 'topCenter',
                    type: 'information',
                    dismissQueue: true,
                    timeout: 4000,
                    modal: true,
                    callback: {
                        onShow: function () { },
                        afterShow: function () { },
                        onClose: function () { notyClosed(message) },
                        afterClose: function () { }
                    },
                    maxVisible: 10
                });
            }
        },
        setJSError: function (message) {
            if (msg != message) {
                msg = message;
                var n = noty({
                    text: message,
                    theme: 'bootstrap',
                    dismissQueue: true,
                    type: 'error',
                    layout: 'topCenter',
                    modal: true,
                    timeout: 6000,
                    callback: {
                        onShow: function () { },
                        afterShow: function () { },
                        onClose: function () { notyClosed(message) },
                        afterClose: function () { }
                    },
                    maxVisible: 10
                });
            }
        },
        notifyUser: function () {
            $.ajax({
                type: 'POST',
                url: path + "Notification/jsonNotifyUser",
                data: {},
                cache: false,
                dataType: 'json',
                success: function (data) {
                    if (data.success != null) {
                        Layout.setJSAlert(data.success, "alert-success");
                    }
                    if (data.info != null) {
                        Layout.setJSAlert(data.info, "alert-info");
                    }
                    if (data.warning != null) {
                        Layout.setJSAlert(data.warning, "alert-warning");
                    }
                    if (data.danger != null) {
                        Layout.setJSAlert(data.danger, "alert-danger");
                    }
                }
            });
        },
        setJSAlert: function (message, type) {

            if (message != null) {
                $(".alert").removeClass("alert-success").removeClass("alert-info").removeClass("alert-warning").removeClass("alert-danger");
                $(".alert").addClass(type);
                $(".alert").html(message);
                $(".alert").show();
                animateScrollTop(0, 1000);
            }
        },

        notyClosed: function (message) {
            if (msg == message) {
                msg = null;
            }
        },

        PrepareCategoriesMenu: function () {
            var fullCollapse = null;
            if ($(document).width() < 768) {
                fullCollapse = true;
            }
            else {
                fullCollapse = false;
            }

            menuCollapse = '#menu-right';
            $('#menu').multilevelpushmenu({
                menu: [JSON.parse(root_categories)],
                //containersToPush: [$('#wrapper'), $('footer')],
                mode: 'cover',
                //fullCollapse: false,                                       // set to true to fully hide base level holder when collapsed
                swipe: 'both',
                backText: '',                                          // Text for 'Back' menu item.
                menuWidth: '280px',
                //menuHeight: '100%',
                backItemIcon: 'fa fa-arrow-circle-right',                         // FontAvesome icon used for back menu item.
                groupIcon: 'fa fa-arrow-circle-left',
                //menuWidth: '250px', // '450px', '30em', '25%' will also work
                collapsed: true,
                fullCollapse: fullCollapse,
                preventItemClick: true,
                preventGroupItemClick: true,
                onExpandMenuStart: function () {
                    $(".levelHolderClass.ltr > ul").css("display", "block");
                    $("#menu-right").multilevelpushmenu('collapse');
                },
                onExpandMenuEnd: function () {
                    $(".b2c-brand").attr('onclick', 'return false;');
                    $(".b2c-brand").css('cursor', 'default');
                    $("#wrapper").css('cursor', 'pointer');

                },
                onCollapseMenuEnd: function () {
                    if ($('#menu').multilevelpushmenu('activemenu').length > 0) {
                        $(".levelHolderClass.ltr > ul").css("display", "block");
                    }
                    else {
                        $(".levelHolderClass.ltr > ul").css("display", "none");
                    }
                    $(".b2c-brand").removeAttr('onclick');
                    $(".b2c-brand").css('cursor', '');

                    $("#wrapper").css('cursor', '');
                },
                onTitleItemClick: function () {
                    var event = arguments[0],
                    // Second argument is menu level object containing clicked item (<div> element)
                    $menuLevelHolder = arguments[1],
                    // Third argument is clicked item (<li> element)
                    $item = arguments[2],
                    // Fourth argument is instance settings/options object
                    options = arguments[3];

                    // You can do some cool stuff here before
                    // redirecting to href location
                    // like logging the event or even
                    // adding some parameters to href, etc...

                    // Anchor href
                    var id = $menuLevelHolder.find('h2:first').data('link');
                    if (typeof (id) != typeof (undefined)) {
                        var menuLevelHolderHref = $menuLevelHolder.find('h2:first').data('link').replace('#', '');
                        $('#CategoryID').val(menuLevelHolderHref);
                        $('#category-form').submit();
                    }

                },
                onItemClick: function () {
                    // First argument is original event object
                    var event = arguments[0],
                        // Second argument is menu level object containing clicked item (<div> element)
                        $menuLevelHolder = arguments[1],
                        // Third argument is clicked item (<li> element)
                        $item = arguments[2],
                        // Fourth argument is instance settings/options object
                        options = arguments[3];

                    // You can do some cool stuff here before
                    // redirecting to href location
                    // like logging the event or even
                    // adding some parameters to href, etc...

                    // Anchor href
                    var itemHref = $item.find('a:first').attr('href').replace('#', '');
                    $('#CategoryID').val(itemHref);
                    $('#category-form').submit();
                },
                onGroupItemClick: function () {
                    var itemHref = arguments[2].find('a:first').attr('href');
                    element = arguments[2];
                    if ($(element).find('li').length == 0) {
                        $.ajax(
                            {
                                type: 'POST',
                                url: path + "Base/jsonGetItemCategories",
                                async: false,
                                data: { CategoryOid: itemHref.replace('#', '') },
                                cache: false,
                                dataType: 'json',
                                success: function (data) {
                                    $('#menu').multilevelpushmenu('additems', JSON.parse(data.result).items, $(element).find('div').first(), 0);
                                }
                            }
                            );
                    }
                },

            });

        },
        RedrawCategoryMenu: function () {

            $('#menu').multilevelpushmenu('option', 'menuHeight', $("#wrapper").height() + $("footer").height());

            $('#menu').multilevelpushmenu('redraw');

        },
        RedrawShoppingCartMenu: function () {

            $('#menu-right').multilevelpushmenu('option', 'menuHeight', $("#wrapper").height() + $("footer").height());

            $('#menu-right').multilevelpushmenu('redraw');
        },
        ShowLoadingPanel: function () {
            LoadingPanel.Show();
        },
        HideLoadingPanel: function () {
            LoadingPanel.Hide();
        },
        RightSidebarRefresh: function () {
            Layout.notifyUser();
            if ($("#callbackList").length != 0) {
                callbackList.PerformCallback();
            }
            if ($("#callbackTableList").length != 0) {
                callbackTableList.PerformCallback();
            }
            if ($("#callbackWishList").length != 0) {
                callbackWishList.PerformCallback();
            }
            $(".cart").click();
        }
    };
})();


function ShoppingCart() {
    var fullCollapse = null;
    if($(document).width() < 768){
        menuWidth = '320px';
        fullCollapse = true;
    }
    else{
        menuWidth = '500px';
        fullCollapse = false;
    }

    menuCollapse = '#menu';  

    $('#menu-right').multilevelpushmenu({
        //containersToPush: [$('#wrapper'), $('footer')],
        mode: 'cover',
        //fullCollapse: false,                                       // set to true to fully hide base level holder when collapsed
        swipe: 'both',
        backText: '',                                          // Text for 'Back' menu item.
        menuWidth: menuWidth,
        //menuHeight: '20%',
        backItemIcon: '',                         // FontAvesome icon used for back menu item.
        groupIcon: '',
        //menuWidth: '250px', // '450px', '30em', '25%' will also work
        collapsed: true,
        direction: 'rtl',
        fullCollapse: fullCollapse,
        preventItemClick: true,
        //onExpandMenuStart: ExpandMenuStart(menuCollapse),
        onExpandMenuStart: function(){
            $("#menu").multilevelpushmenu('collapse');
        },
        onItemClick: function () {
            // First argument is original event object
            var event = arguments[0],
                // Second argument is menu level object containing clicked item (<div> element)
                $menuLevelHolder = arguments[1],
                // Third argument is clicked item (<li> element)
                $item = arguments[2],
                // Fourth argument is instance settings/options object
                options = arguments[3];

            // You can do some cool stuff here before
            // redirecting to href location
            // like logging the event or even
            // adding some parameters to href, etc...

            // Anchor href
            var itemHref = $item.find('a:first').attr('href');
            // Redirecting the page
            location.href = path + itemHref;
        },

    });
    Layout.RedrawShoppingCartMenu();
    Layout.RedrawCategoryMenu();
}
   

function ExpandMenuStart(menu) {

    if ($(menu + " .levelHolderClass").hasClass("multilevelpushmenu_inactive") == false) {
        $(menu).multilevelpushmenu('collapse');
    }
}


function animateScrollTop(target, duration) {
    duration = duration || 16;

    var $window = $(window);
    var scrollTopProxy = { value: $window.scrollTop() };
    var expectedScrollTop = scrollTopProxy.value;

    if (scrollTopProxy.value != target) {
        $(scrollTopProxy).animate(
            { value: target },
            {
                duration: duration,

                step: function (stepValue) {
                    var roundedValue = Math.round(stepValue);
                    if ($window.scrollTop() !== expectedScrollTop) {
                        // The user has tried to scroll the page
                        $(scrollTopProxy).stop();
                    }
                    $window.scrollTop(roundedValue);
                    expectedScrollTop = roundedValue;
                },

                complete: function () {
                    if ($window.scrollTop() != target) {
                        setTimeout(function () {
                            animateScrollTop(target);
                        }, 16);
                    }
                }
            }
        );
    }
}