$(function () {
    // Commodity Autocomplete
    $("#commodityInput").autocomplete({
        minLength: 1,
        source: function (request, response) {

            if (!request.term || request.term.length < 1) return;

            $.ajax({
                url: '/Rot/SearchCommodity',
                data: { term: request.term },
                success: function (data) {

                    console.log("DATA:", data);

                    data.forEach(item => {
                        console.log(item.id + " - " + item.label);
                    });

                    response(data);
                },
                error: function (err) {
                    console.log("ERROR:", err);
                }
            });
        },
        select: function (event, ui) {
            $("#commodityInput").val(ui.item.label);
            $("#commodityId").val(ui.item.id);
            return false;
        }
    });
});

$(document).ready(function () {

    // 🔹 Surcharge Dropdown
    var dropdown = $('#TariffLocationDropdown');

    $.ajax({
        url: '/Rot/GetSurchargeList',
        type: 'GET',
        success: function (data) {

            dropdown.empty();
            dropdown.append('<option value="all">Select All</option>');

            $.each(data, function (i, item) {
                dropdown.append(
                    $('<option>', {
                        value: item.id,
                        text: item.text
                    })
                );
            });

            dropdown.select2({
                placeholder: "Select Tariff Surcharge",
                width: '100%'
            });

            dropdown.on('change', function () {

                var values = $(this).val();

                if (values && values.includes('all')) {

                    var allValues = [];

                    $('#TariffLocationDropdown option').each(function () {
                        var val = $(this).val();

                        if (val !== 'all') {
                            allValues.push(val);
                        }
                    });

                    dropdown.val(allValues).trigger('change');
                }
            });
        }
    });

    // 🔹 Service Type Dropdown
    $.ajax({
        url: '/Rot/GetServiceTypes',
        type: 'GET',
        success: function (data) {

            var serviceDropdown = $('#ServiceTypeDropdown');
            serviceDropdown.empty();

            serviceDropdown.append('<option value="">Select Service Type</option>');

            $.each(data, function (i, item) {
                serviceDropdown.append(
                    $('<option>', {
                        value: item.value,
                        text: item.text
                    })
                );
            });
        }
    });

});