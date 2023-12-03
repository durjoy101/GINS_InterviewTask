
    $(document).ready(function () {
        var customerApiEndpoint = 'http://localhost:5072/api/MeetingDetails/customerName?Type=';
        var productApiEndpoint = 'http://localhost:5072/api/MeetingDetails/productService';
        var saveMeetingMinutesApiEndpoint = 'http://localhost:5072/api/MeetingDetails/saveMeetingMinutes';

        var test = $('input[name="inlineRadioOptions"]:checked').val();
        var rowCount = 0;

        $('input[name="inlineRadioOptions"]').change(function () {
            fetchCustomerData();
        });

        $('#interestedProduct').change(function () {
            var selectedProductName = $(this).val();

            if (selectedProductName !== '') {
                var selectedProduct = productData.find(item => item.product === selectedProductName);
                $('#unit').val(selectedProduct.unit);
            } else {
                $('#unit').val('');
            }
        });

        function populateCustomerDropdown(customerData) {
            var dropdown = $('#customerDropdown');

            dropdown.empty();

            // Add default option
            dropdown.append('<option value="">select customer name</option>');

            $.each(customerData, function (index, item) {
                dropdown.append('<option value="' + item.customerName + '">' + item.customerName + '</option>');
            });
        }

        function fetchProductData() {
            $.ajax({
                url: productApiEndpoint,
                method: 'GET',
                dataType: 'json',
                success: function (data) {
                    productData = data;

                    populateProductDropdown();
                },
                error: function (error) {
                    console.error('Error fetching product data:', error);
                }
            });
        }

        function fetchCustomerData() {
            var radioValue = $('input[name="inlineRadioOptions"]:checked').val();

            var updatedCustomerApiEndpoint = customerApiEndpoint + radioValue;

            $.ajax({
                url: updatedCustomerApiEndpoint,
                method: 'GET',
                dataType: 'json',
                success: function (customerData) {
                    // Populate the customer dropdown with fetched data
                    populateCustomerDropdown(customerData);
                },
                error: function (error) {
                    console.error('Error fetching customer data:', error);
                }
            });
        }

        function populateProductDropdown() {
            var dropdown = $('#interestedProduct');

            dropdown.empty();

            dropdown.append('<option disabled value="" selected>select product/service inserted</option>');

            $.each(productData, function (index, item) {
                dropdown.append('<option value="' + item.product + '">' + item.product + '</option>');
            });
        }

        

        $('#addButton').click(function () {
            addRowToTable();
        });

        $('#dataTable').on('click', '.delete-button', function () {
            deleteRow(this);
        });

        function addRowToTable() {

            rowCount++;

            var productName = $('#interestedProduct').val();
            var quantity = $('#quantity').val();
            var unit = $('#unit').val();

            var newRow = '<tr>' +
                '<td class="serial-number">' + rowCount + '</td>' +
                '<td class="product-service">' + productName + '</td>' +
                '<td class="quantity">' + quantity + '</td>' +
                '<td class="unit">' + unit + '</td>' +
                '<td><button class="btn btn-warning btn-sm">Edit</button></td>' +
                '<td><button class="btn btn-danger btn-sm delete-button">Delete</button></td>' +
                '</tr>';

            if (rowCount == 1) {
                $('#dataTable tbody tr').remove();
            }


            $('#dataTable tbody').append(newRow);

            $('#interestedProduct').val('');
            $('#quantity').val('');
            $('#unit').val('');
        }

        function deleteRow(button) {
            var row = $(button).closest('tr');

            row.remove();

            updateSerialNumbers();
        }

        function updateSerialNumbers() {
            $('#dataTable tbody tr').each(function (index) {
                $(this).find('.serial-number').text(index + 1);
            });

            rowCount = $('#dataTable tbody tr').length;
            if (rowCount == 0) {
                var newRow = '<tr class="text-center">' +
                    '<td colspan="6" class="align-middle">' + 'No matching record found.' + '</td>' +
                    '</tr>';
                $('#dataTable tbody').append(newRow);
            }
        }



        // Add an event listener to the form submit
        $('#meetingMinutesForm').submit(function (event) {

            $('.is-invalid').removeClass('is-invalid');
            event.preventDefault();

            var formData = {
                CustomerType: $('input[name="inlineRadioOptions"]:checked').val(),
                CustomerName: $('#customerDropdown option:selected').text(),
                MeetingDate: $('input[name="MeetingDate"]').val(), 
                MeetingPlace: $('input[name="MeetingPlace"]').val(), 
                AttendsClient: $('textarea[name="AttendsClient"]').val(), 
                AttendsHost: $('textarea[name="AttendsHost"]').val(), 
                MeetingAgenda: $('input[name="MeetingAgenda"]').val(), 
                MeetingDiscussion: $('textarea[name="MeetingDiscussion"]').val(), 
                MeetingDecision: $('textarea[name="MeetingDecision"]').val(), 
                MeetingMunitesProducts: getTableRowsData()
            };

            $.ajax({
                type: 'POST',
                url: saveMeetingMinutesApiEndpoint, 
                contentType: 'application/json',
                data: JSON.stringify(formData),
                success: function (data) {
                    alert(data);
                },
                error: function (error) {
                    console.error(error);
                }
            });
        });

        function getTableRowsData() {
            var tableRowsData = [];

            $('#dataTable tbody tr:not(.no-data)').each(function () {
                var rowData = {
                    ProductService: $(this).find('.product-service').text(),
                    Quantity: $(this).find('.quantity').text(),
                    Unit: $(this).find('.unit').text()
                };

                tableRowsData.push(rowData);
            });

            return tableRowsData;
        }

        $('#saveButton').click(function () {
            if (validateForm()) {
                $('#meetingMinutesForm').submit();
            } else {
                alert('Please fill in all required fields.');
            }
        });

        function validateForm() {
            var isValid = true;
            $('.form-control').removeClass('is-invalid');

            if ($('#customerDropdown').val() === '') {
                $('#customerDropdown').addClass('is-invalid');
                isValid = false;
            }

            if ($('input[name="MeetingAgenda"]').val() === '') {
                $('input[name="MeetingAgenda"]').addClass('is-invalid');
                isValid = false;
            }
            if ($('textarea[name="MeetingDiscussion"]').val() === '') {
                $('textarea[name="MeetingDiscussion"]').addClass('is-invalid');
                isValid = false;
            }
            if ($('textarea[name="MeetingDecision"]').val() === '') {
                $('textarea[name="MeetingDecision"]').addClass('is-invalid');
                isValid = false;
            }
            if ($('input[name="MeetingPlace"]').val() === '') {
                $('input[name="MeetingPlace"]').addClass('is-invalid');
                isValid = false;
            }

            if ($('textarea[name="AttendsClient"]').val() === '') {
                $('textarea[name="AttendsClient"]').addClass('is-invalid');
                isValid = false;
            }

            if ($('textarea[name="AttendsHost"]').val() === '') {
                $('textarea[name="AttendsHost"]').addClass('is-invalid');
                isValid = false;
            }


            return isValid;
        }

        $('#refreshButton').click(function () {
            location.reload();
        });

        fetchProductData();
        fetchCustomerData();
    });
