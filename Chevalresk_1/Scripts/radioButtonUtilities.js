$(document).ready(function () {
    $("#typeItem_3").click(function () {

        if ($("#typeItem_3").is(":checked")) {
            $("#Ressource").show();
            $("#Potion").hide();
            $("#Armure").hide();
            $("#Arme").hide();
        }
    })
    $("#typeItem_2").click(function () {

        if ($("#typeItem_2").is(":checked")) {
            $("#Potion").show();
            $("#Arme").hide();
            $("#Armure").hide();
            $("#Ressource").hide();
        }
    })
    $("#typeItem_1").click(function () {

        if ($("#typeItem_1").is(":checked")) {
            $("#Armure").show();
            $("#Arme").hide();
            $("#Potion").hide();
            $("#Ressource").hide();
        }
    })
    $("#typeItem_0").click(function () {

        if ($("#typeItem_0").is(":checked")) {
            $("#Arme").show();
            $("#Armure").hide();
            $("#Potion").hide();
            $("#Ressource").hide();
        }
    })
});