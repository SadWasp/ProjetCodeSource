$(document).ready(function () {
    $("#Type_3").click(function () {

        if ($("#Type_3").is(":checked")) {
            $("#Ressource").show();
            $("#Potion").hide();
            $("#Armure").hide();
            $("#Arme").hide();
            document.getElementById("effet").setAttribute("value", "Poison");
            document.getElementById("matiere").setAttribute("value", "Bois");
            document.getElementById("efficaciteArme").setAttribute("value", "1");
            document.getElementById("description").setAttribute("value", "");
        }
    })
    $("#Type_2").click(function () {

        if ($("#Type_2").is(":checked")) {
            $("#Potion").show();
            $("#Arme").hide();
            $("#Armure").hide();
            $("#Ressource").hide();
            document.getElementById("description").setAttribute("value", "une description");
            document.getElementById("matiere").setAttribute("value", "Bois");
            document.getElementById("efficaciteArme").setAttribute("value", "1");
            document.getElementById("effet").setAttribute("value", "");
        }
    })
    $("#Type_1").click(function () {

        if ($("#Type_1").is(":checked")) {
            $("#Armure").show();
            $("#Arme").hide();
            $("#Potion").hide();
            $("#Ressource").hide();
            document.getElementById("effet").setAttribute("value", "Poison");
            document.getElementById("description").setAttribute("value", "une description");
            document.getElementById("efficaciteArme").setAttribute("value", "1");
            document.getElementById("matiere").setAttribute("value", "");
        }
    })
    $("#Type_0").click(function () {

        if ($("#Type_0").is(":checked")) {
            $("#Arme").show();
            $("#Armure").hide();
            $("#Potion").hide();
            $("#Ressource").hide();
            $.validator.addMethod(
                "genreArme",
                function (value, element) {
                    return ($("input[name='genreArme']:checked").val() != undefined);
                },
                "Choix obligatoire"
            );
            document.getElementById("effet").setAttribute("value", "Poison");
            document.getElementById("matiere").setAttribute("value", "Bois");
            document.getElementById("description").setAttribute("value", "une description");
            document.getElementById("efficaciteArme").setAttribute("value", "");
        }
    })
});