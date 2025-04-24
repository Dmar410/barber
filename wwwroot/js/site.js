document.getElementById("selectedDate").addEventListener("input", function() {
    let selectedDate = new Date(this.value);
    let day = selectedDate.getDay();
    
    if (day === 0 || day === 1) { // 0 = Sunday, 1 = Monday
        alert("Appointments are not available on Sundays and Mondays.");
        this.value = "";
    }
});
