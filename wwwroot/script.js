document.addEventListener("DOMContentLoaded", function () {
    const appointmentForm = document.getElementById("appointment-form");
    const dateInput = document.getElementById("date");
    const timeSelect = document.getElementById("time");

    // Disabled days (Monday = 1, Tuesday = 2)
    function isClosedDay(date) {
        const day = new Date(date).getDay();
        return day === 1 || day === 2;
    }

    // Generate available times
    function generateAvailableTimes(selectedDate) {
        timeSelect.innerHTML = "";
        if (isClosedDay(selectedDate)) {
            alert("The barbershop is closed on Mondays and Tuesdays.");
            return;
        }

        let allTimes = [];
        for (let hour = 9; hour < 18; hour++) {
            allTimes.push(`${hour}:00`, `${hour}:30`);
        }

        let bookedTimes = JSON.parse(localStorage.getItem(selectedDate)) || [];

        let availableTimes = allTimes.filter(time => {
            let [hour, minute] = time.split(":").map(Number);
            let currentTime = new Date(selectedDate);
            currentTime.setHours(hour, minute, 0, 0);

            // Check if the selected time is booked
            return !bookedTimes.some(booked => {
                let bookedTime = new Date(booked);
                let diff = (currentTime - bookedTime) / (1000 * 60); // Get difference in minutes

                return diff >= 0 && diff <= 75; // Block 1 hour + 15 min break
            });
        });

        if (availableTimes.length === 0) {
            alert("No available times left for this day.");
        } else {
            availableTimes.forEach(time => {
                let option = document.createElement("option");
                option.value = time;
                option.textContent = time;
                timeSelect.appendChild(option);
            });
        }
    }

    // Load times on date change
    dateInput.addEventListener("change", function () {
        generateAvailableTimes(this.value);
    });

    // Handle form submission
    appointmentForm.addEventListener("submit", function (event) {
        event.preventDefault();
        let selectedDate = dateInput.value;
        let selectedTime = timeSelect.value;
        let email = document.getElementById("email").value;

        if (!selectedDate || !selectedTime || !email) {
            alert("Please fill in all fields.");
            return;
        }

        let bookedTimes = JSON.parse(localStorage.getItem(selectedDate)) || [];
        bookedTimes.push(new Date(`${selectedDate}T${selectedTime}:00`).toISOString());

        localStorage.setItem(selectedDate, JSON.stringify(bookedTimes));

        alert("Appointment booked successfully!");
        window.location.href = "index.html"; // Redirect back to home
    });
});
