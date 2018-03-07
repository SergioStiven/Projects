using System;

namespace Xpinn.SportsGo.Util.Portable
{
    public static class AppConstants
    {
        // ================================================
        //          Variables Hockey App Android
        // ================================================
        // SportsGo
        //public const string IdHockeyAppAndroid = "e6521b82f43942bab9281c434a8f91fa";

        // SportsGoPruebas
        //public const string IdHockeyAppAndroid = "7f3681dfe5214e38be644cc38062759c";

        // SportsGoProduccion
        //public const string IdHockeyAppAndroid = "d78466f76d424c4a9fc68a9277007059";

        // SportsGoPruebasProduccion
        public const string IdHockeyAppAndroid = "f7aced6016d94e5c8a94a36de70a55a6";

        // ================================================
        //          Variables Hockey App iOS
        // ================================================
        // SportsGo
        //public const string IdHockeyAppiOS = "a715fef6c1134455a8a9f5ecb6b96a6f";

        // SportsGoPruebas
        //public const string IdHockeyAppiOS = "5db5cefda2bb4e659f6f54750c4aa136";

        // SportsGoProduccion
        //public const string IdHockeyAppiOS = "dc9565f05af44a549e388e80f8229640";

        // SportsGoPruebasProduccion
        public const string IdHockeyAppiOS = "b457d64555ac428c87600a40221d447a";

        // Informacion para FireBase Console
        public const string ServerAuthKeyFireBase = @"AAAA41FZ5CM:APA91bHvIVUizzhIhXno0_OPmKDk5OF5y7uvMSoFfg65PuAM4XXGIAQEIXdNbVXZLdV5YiIDmCf7aBM9WmV31IFoIspT05bISoDd4DXW2C6qARtOGxoGhlTB-WU5zP2X63IXq9WMHHpX";

        // Informacion para el correo de la aplicacion
        public const string CorreoAplicacion = "sportsgosoporte@gmail.com";
        public const string ClaveCorreoAplicacion = "T1m32014*";

        // Placeholder para el formato del correo
        public const string PlaceHolderNombre = "@_Nombre_@";
        public const string PlaceHolderUsuario = "@_Usuario_@";
        public const string PlaceHolderClave = "@_Contraseña_@";
        public const string PlaceHolderImagenLogo = "@_ImagenLogo_@";
        public const string PlaceHolderImagenBanner = "@_ImagenBanner_@";
        public const string PlaceHolderUrlWeb = "@_UrlWeb_@";
        public const string PlaceHolderUrlPaginaConfirmacion = "@_Url_Pagina_Confirmacion_@";

        // Placeholder para formato de las notificaciones
        public const string PlaceNombrePersona = "@_NombrePersona_@";
        public const string PlaceTituloEvento = "@_TituloEvento_@";
        public const string PlaceNombrePlan = "@_NombrePlan_@";
        public const string PlaceFechaVencimientoPlan = "@_FechaVencimientoPlan_@";

        // Formatos de fechas, maximo y minimo fechas permitidas
        public const string DateFormat = "dd/MM/yyyy";
        public static readonly DateTime MinimumDate = new DateTime(1950, 1, 1);
        public static readonly DateTime MaximumDate = new DateTime(2050, 1, 1);

        // Mayoria de edad
        public const int MayoriaEdad = 18;

        // Configuracion video
        public const int MinimoSegundos = 1;
        public const int MaximoSegundos = 600;

        public const string RegexEmail = @"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9]+\.[a-zA-Z]{2,5}(\.[a-zA-Z]{2,5}){0,1}";
        public const string RegexUserName = @"^[a-zA-Z0-9_.-]*$";
        public const string RegexPassword = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d]{8,}$";

        //public const string PayUURL = "https://sandbox.gateway.payulatam.com/ppp-web-gateway";
        public const string PayUURL = "https://gateway.payulatam.com/ppp-web-gateway";

        //public const string PayUApiKey = "4Vj8eK4rloUd272L48hsrarnUA";
        public const string PayUApiKey = "zH3brw3eazrjKbshl1qyRw26aV";

        //public const string PayUMerchantID = "508029";
        public const string PayUMerchantID = "653460";

        //public const string PayUAccountID = "512321";
        public const string PayUAccountID = "655964";

        public const int PayUTest = 1;
    }
}
