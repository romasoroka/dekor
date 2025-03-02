document.getElementById("callRequestForm").addEventListener("submit", function(event) {
    event.preventDefault(); 

let name = document.getElementById("clientName").value;
let phone = document.getElementById("clientPhone").value;

if (!name || !phone) {
    alert("Будь ласка, заповніть усі поля!");
return;
    }

let botToken = "8068878790:AAGZnuBNgS7yLGaFQ9Oav-ShM2LjdIJUep4";
let chatId = "565791117";
let message = `📞 Нова заявка на дзвінок!\n👤 Ім'я: ${name}\n📱 Телефон: ${phone}`;

let telegramUrl = `https://api.telegram.org/bot${botToken}/sendMessage?chat_id=${chatId}&text=${encodeURIComponent(message)}`;

fetch(telegramUrl)
        .then(response => {
            if (response.ok) {
    alert("Заявка відправлена!");
document.getElementById("callRequestForm").reset(); 
var modal = bootstrap.Modal.getInstance(document.getElementById('callRequestModal'));
modal.hide(); 
            } else {
    alert("Помилка при відправці заявки!");
            }
        })
        .catch(error => {
    console.error("Помилка:", error);
alert("Не вдалося відправити заявку!");
        });
});
