const send = (url, jsonstr) => {
    fetch(url, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            proto
        })
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                return data;
            }
            else {
                return data;
            }
        })
        .catch(error => ('Error:', error));
};

$('.toggle-button input').each((i, checkbox) => {
    $(checkbox).change(function () {
        const SamAccountName = $(this).data('userid'),
            isChecked = this.checked;

        const url = '/RemoteAccess/DisableDialIn';

        fetch(url, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'X-CSRF-TOKEN': $('input[name="__RequestVerificationToken"]').val()
            },
            body: JSON.stringify({ SamAccountName: SamAccountName, isEnabled: isChecked })
        })

            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    console.log(data)

                    updatePage(data.updatedData.samAccountName, data.updatedData.isDialInEnabled);

                    console.log(data);
                }
                else {
                    console.error('Ошибка обновления состояния в контроллере пользователя')
                }
            })
            .catch(error => console.error('Error:', error));
    })
});


function updatePage(SamAccountName, IsDialInEnabled) {

    // Обновляем данные пользователя на странице
    $(`[data-userid="${SamAccountName}"]`).text(IsDialInEnabled);
    $(`[data-status="${SamAccountName}"]`).text(IsDialInEnabled ? "Активен" : "Выключен");
}

(function SendFile() {
    const url = '/RemoteAccess/UploadFile';
    $('.FormDataARA').on('submit', function (e) {
        e.preventDefault();
        const form = $(this);

        let fileInput = form.find('.FormDataARA__inputFile')[0];
        let file = fileInput.files[0];

        let formData = new FormData();
        formData.append('file', file);

        fetch(url, {
            method: 'POST',
            body: formData
        })
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    console.log(data);
                } else {
                    console.error(data);
                }
            })
            .catch(error => console.error('Error:', error));
    });
})();

(function addUserToRA() {
    const url = '/RemoteAccess/AddUserToRA';
    $('.form__autoRA').on('submit', function () {
        const form = $(this);
        const proto = {};
        form.find('#DateOfDisconnect, #selectUser').each((i, el) => {
            el = $(el);
            proto[el.attr('id')] = el.val();
        });

        console.log(proto);
        
        fetch(url, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                DateOfDisconnect: proto.DateOfDisconnect,
                selectUser: proto.selectUser
            })
        })
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    return data;
                    console.log(2);
                }
                else {
                    return data;
                    console.log(1);
                }
            })
            .catch(error => ('Error:', error));

        console.log(JSON.stringify({
            DateOfDisconnect: proto.DateOfDisconnect,
            selectUser: proto.selectUser
        }));

        return false;
    });
}());

(function addNewUser() {
    const url = '/User/CreateUser';
    $('#Form').on('submit', e => {

        const form = $(e.delegateTarget);
        const proto = {};

        form.find('.form__input').each((i, el) => {
            el = $(el);
            proto[el.attr('id')] = el.val();
        });

        proto.ou = form.find('.form__radioInput:checked').val();

        console.log(send(url, proto));

        return false;
    });
}());

/*

let distinguishedName = '';
let boxes = []

$('.list-group-item input').each((i, checkbox) => {
    boxes.push(checkbox)
    $(checkbox).change(function () {
        if (checkbox.checked == true) {
            distinguishedName = $(this).data('path');

            for (i = 0; i < boxes.length; i++) {
                if ($(boxes[i]).data('path') != distinguishedName) {
                    boxes[i].checked = false;
                }
            }

            console.log(distinguishedName);
        }
        if (checkbox.checked == false) {
            distinguishedName = "";
        }
    });
});

function valid(textbox) {
    if (textbox.value != "") return true
    else return false
}

document.getElementById('addNewUser').addEventListener('click', function () {

    const url = '/User/CreateUser';

    if (distinguishedName != "" && valid(document.getElementById('GivenName'))
        && valid(document.getElementById('Surname')) && valid(document.getElementById('SamAccountName'))
        && valid(document.getElementById('Password')))
    {
        fetch(url, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                GivenName: document.getElementById('GivenName').value, Surname: document.getElementById('Surname').value,
                SamAccountName: document.getElementById('SamAccountName').value, Password: document.getElementById('Password').value,
                DistinguishedName: distinguishedName
            })
        })
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    console.log(data);
                }
                else {
                    console.error(data)
                }
            })
            .catch(error => console.error('Error:', error));
    }
    else {
        alert('ПОЛЯ ЗАПОЛНЕНЫ НЕПРАВИЛЬНО!!!!!!!')
    }
})

(function () {
    fetch('/User/GetUsers', {
        method: "GET",
        headers: {
            'Content-Type': 'application/json',
        }
    })
        .then(response => response.json())
        .then(data => {
            GetUsers(data);
        });
}());

function GetUsers(organisationUnit) {
    const target = $('#organisationBody');
    organisationUnit.forEach(ou => {
        ou.Users.forEach(u => {
            const tr = $('<tr></tr>');
            tr.append(`<td>${u.GivenName}</td>`)
            tr.append(`<td>${u.Surname}</td>`)
            tr.append(`<td>${u.SamAccountName}</td>`)
            tr.append(`<td>${u.IsDialInEnabled}</td>`)
            tr.append(`<td><input type="checkbox id="toggle-${u.SamAccountName}"></td>`)

            tr.appendTo(target);
        });
        if (ou.Children.length > 0) {
            GetUsers(ou.Children);
        }
    })

}

function drawAll(organisationUnits) {
    const tableBody = document.getElementById("organisationsBody");

    organisationUnits.forEach(organisation => {
        const row = document.createElement("tr");

        const cell1 = document.createElement("td");
        cell1.textContent = "Узел " + organisation.Name;
        row.appendChild(cell1);

        const cell2 = document.createElement("td");
        cell2.textContent = "Узлов внутри " + organisation.Children.length;
        row.appendChild(cell2);

        tableBody.appendChild(row);

        organisation.Users.forEach(user => {
            const userRow = document.createElement("tr");

            const cell1 = document.createElement("td");
            const cell2 = document.createElement("td");
            const userCell = document.createElement("td");
            userCell.textContent = "Пользователь " + user.GivenName + " Узел " + organisation.Name;
            userRow.appendChild(cell1);
            userRow.appendChild(cell2);
            userRow.appendChild(userCell);

            tableBody.appendChild(userRow);
        });

        if (organisation.Children.length > 0) {
            drawAll(organisation.Children);
        }
    });
}


document.querySelectorAll('.toggle-button input').forEach(function (checkbox) {
    checkbox.addEventListener('change', function () {
        var SamAccountName = $(this).data('userid');
        console.log(SamAccountName)
        var isChecked = this.checked;

        AJAX-запрос для изменения состояния Dial-in на сервере
        var url = '/User/DisableDialIn';

        fetch(url, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'X-CSRF-TOKEN': $('input[name="__RequestVerificationToken"]').val()
            },
            body: JSON.stringify({ SamAccountName: SamAccountName, isEnabled: isChecked })
        })

            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    console.log(data)
                    Получаем данные и обновляем страницу
                    updatePage(data.updatedData.samAccountName, data.updatedData.isDialInEnabled);

                    console.log(data);
                }
                else {
                    console.error('Ошибка обновления состояния в контроллере пользователя')
                }
            })
            .catch(error => console.error('Error:', error));
    });
});

function updatePage(SamAccountName, IsDialInEnabled) {

     Обновляем данные пользователя на странице
    document.querySelector(`[data-userid="${SamAccountName}"]`).innerText = IsDialInEnabled;
}
*/