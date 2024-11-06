using ITS.Retail.WebClient.AuxillaryClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace ITS.Retail.WebClient
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {

#if DEBUG
            BundleTable.EnableOptimizations = false;
#else
            BundleTable.EnableOptimizations = true;
#endif
            bundles.Add(
                new ScriptBundle("~/bundles/mainpage.js").Include(
                "~/Scripts/jquery-1.11.1.js",
                "~/Scripts/scripts/jquery.cookie.js",
                "~/Scripts/jquery-ui.js",
                "~/Scripts/scripts/GridContextMenu.js",
                "~/Scripts/jquery.resizeimagetoparent.min.js",
                "~/Scripts/scripts/Helpers.js",
                "~/Scripts/scripts/Layout.js",
                "~/Scripts/noty/jquery.noty.js",
                "~/Scripts/noty/layouts/bottomRight.js",
                "~/Scripts/noty/layouts/topCenter.js",
                "~/Scripts/noty/themes/default.js",
                "~/Scripts/modernizr.js",
                "~/Scripts/scripts/jquery.modaldialog.js",
                "~/Scripts/jquery.unobtrusive-ajax.js",
                "~/Scripts/scripts/OrderItems.js",
                "~/Scripts/jquery.tagcloud.js",
                "~/Scripts/scripts/MarkUp.js",
                "~/Scripts/scripts/Document.js",
                "~/Scripts/jquery.custom-scrollbar.min.js",
                "~/Scripts/scripts/EditDocument.js",
                "~/Scripts/scripts/EditDocument.js",
                "~/Scripts/scripts/DocumentCommon.js",
                "~/Scripts/jquery.validate-old.js",
                "~/Scripts/jquery.validate.unobtrusive-old.js",
                "~/Scripts/customvalidation.js",
                "~/Scripts/respond.min.js",
                "~/Scripts/scripts/VariableValuesDisplay.js",
                "~/Scripts/build/client.bundle.js",
                "~/Scripts/bootstrap.js"
                ));

            //SignalR
            bundles.Add(new ScriptBundle("~/bundles/SignalR.js").Include(
               "~/Scripts/jquery.signalR-1.2.2.min.js",
               //"~/signalr/hubs",
               //"~/Scripts/scripts/SignalR.min.js"
               "~/Scripts/scripts/SignalR.js"
               ));


            bundles.Add(new ScriptBundle("~/bundles/editdocumentextras.js").Include(
               "~/Scripts/scripts/Transformation.js",
               "~/Scripts/scripts/DocumentOnly.js"
               ));


            // reports
            bundles.Add(new ScriptBundle("~/bundles/reports.js").Include(
                        "~/Scripts/jquery-1.11.1.js",
                        "~/Scripts/scripts/jquery.cookie.js",
                        "~/Scripts/jquery-ui.js",
                        "~/Scripts/knockout-3.3.0.js",
                        "~/Scripts/globalize.js",
                        "~/Scripts/cultures/globalize.cultures.js",
                        "~/Scripts/respond.min.js"
                       ));

            //B2C
            bundles.Add(new ScriptBundle("~/bundles/B2C/b2cmain.js").Include(
                                         "~/Scripts/jquery-1.11.1.js",
                                         "~/Content/B2C/js/libraries/jquery.cookie.js",
                                         "~/Scripts/globalize.js",
                                         "~/Scripts/cultures/globalize.cultures.js",
                                         "~/Content/B2C/js/Globalize.js",
                                         "~/Content/B2C/js/libraries/jquery.slides.min.js",
                                         "~/Content/B2C/js/libraries/modernizr.js",
                                         "~/Content/B2C/js/libraries/respond.min.js",
                                         "~/Content/B2C/js/libraries/jquery-ui.min.js",
                                         "~/Scripts/jquery.unobtrusive-ajax.js",
                                         "~/Content/B2C/bootstrap-sass-master/bootstrap/javascripts/bootstrap.js",
                                         "~/Content/B2C/js/libraries/multilevel/jquery.multilevelpushmenu.js",
                                         "~/Content/B2C/js/libraries/scrollAppend.js",
                                         "~/Content/B2C/js/Layout.js",
                                         "~/Content/B2C/js/Products.js",
                                         "~/Content/B2C/js/WishList.js",
                                         "~/Content/B2C/js/libraries/jplist/jplist-core.min.js",
                                        "~/Content/B2C/js/libraries/jplist/jplist.views-control.min.js",
                                        "~/Content/B2C/js/libraries/jquery.magnific-popup.min.js"));




            bundles.Add(
                new StyleBundle("~/Content/bundles/mainpage.css")
                .Include("~/Content/css/animate.min.css", new CssRewriteUrlTransformWrapper())
                .Include("~/Content/css/Init.css", new CssRewriteUrlTransformWrapper())
                .Include("~/Content/css/Site.css", new CssRewriteUrlTransformWrapper())
                .Include("~/Content/css/codemirror.css", new CssRewriteUrlTransformWrapper())
                .Include("~/Content/css/3024-day.css", new CssRewriteUrlTransformWrapper())
                .Include("~/Content/font-awesome/css/font-awesome.min.css", new CssRewriteUrlTransformWrapper())
                .Include("~/Content/css/jquery-ui.css", new CssRewriteUrlTransformWrapper())
                .Include("~/Content/css/jquery-ui.css", new CssRewriteUrlTransformWrapper())
                .Include("~/Content/css/Document.css", new CssRewriteUrlTransformWrapper())
                .Include("~/Content/css/jquery.custom-scrollbar.css", new CssRewriteUrlTransformWrapper())
                .Include("~/Content/css/Order.css", new CssRewriteUrlTransformWrapper())
                .Include("~/Content/css/Proforma.css", new CssRewriteUrlTransformWrapper())
                .Include("~/Content/css/bootstrap-table.css", new CssRewriteUrlTransformWrapper())
                .Include("~/Content/css/bootstrap.css", new CssRewriteUrlTransformWrapper())
                .Include("~/Scripts/build/css/style.css", new CssRewriteUrlTransformWrapper())
                .Include("~/Scripts/build/lib/react-select.css", new CssRewriteUrlTransformWrapper())
                .Include("~/Scripts/build/lib/style.css", new CssRewriteUrlTransformWrapper())

                );

            //b2c

            bundles.Add(
                new StyleBundle("~/content/B2C/bundles/b2c.css")
                .Include("~/Content/B2C/bootstrap-sass-master/bootstrap/stylesheets/style.css", new CssRewriteUrlTransformWrapper())
                .Include("~/Content/B2C/css/style.css", new CssRewriteUrlTransformWrapper())
                .Include("~/Content/B2C/css/page.css", new CssRewriteUrlTransformWrapper())
                .Include("~/Content/B2C/css/media.css", new CssRewriteUrlTransformWrapper())
                .Include("~/Content/css/jquery.custom-scrollbar.css", new CssRewriteUrlTransformWrapper())
                .Include("~/Content/B2C/css/loader.css", new CssRewriteUrlTransformWrapper())
                .Include("~/Content/font-awesome/css/font-awesome.min.css", new CssRewriteUrlTransformWrapper())
                .Include("~/Content/B2C/css/multilevel/jquery.multilevelpushmenu.css", new CssRewriteUrlTransformWrapper())
                .Include("~/Content/B2C/css/jplist/jplist-demo-pages.css", new CssRewriteUrlTransformWrapper())
                .Include("~/Content/B2C/css/jplist/jplist-core.min.css", new CssRewriteUrlTransformWrapper())
                .Include("~/Content/B2C/css/jplist/jplist-views-control.min.css") //Υπάρχει γνωστό bug με το Bundling
                .Include("~/Content/B2C/css/magnific/main.css", new CssRewriteUrlTransformWrapper())
                .Include("~/Content/B2C/css/product_listing.css", new CssRewriteUrlTransformWrapper())
                .Include("~/Content/B2C/css/bs-language/languages.min.css", new CssRewriteUrlTransformWrapper())
                );

            //reports

            bundles.Add(
                new StyleBundle("~/content/bundles/reports.css")
                .Include("~/Content/css/Site.css", new CssRewriteUrlTransformWrapper())
                .Include("~/Content/css/Printer_Layout.css", new CssRewriteUrlTransformWrapper())
                .Include("~/Content/css/jquery-ui-1.8.21.custom.css", new CssRewriteUrlTransformWrapper())
                .Include("~/Content/css/Reports.css", new CssRewriteUrlTransformWrapper())
                .Include("~/Content/css/Custom-Reports.css", new CssRewriteUrlTransformWrapper())
                );


#if DEBUG
            foreach (var bundle in BundleTable.Bundles)
            {
                bundle.Transforms.Clear();
            }
#endif


        }
    }
}