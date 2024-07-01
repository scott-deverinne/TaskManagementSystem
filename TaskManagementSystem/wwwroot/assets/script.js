// Ensure that the code isn't run until the browser has finished 
// rendering all the elements in the html.
$(window).on("load", function () {

  // Waits for a "Save Button" to be clicked and then saves the value 
  // entered into the text box to Local Storage. 
  $(".saveBtn").click(function () {
    var time = $(this).parent().attr("id");
    var task = $(this).siblings(".description").val();
    localStorage.setItem(time, task);
  });

  // Applies the past, present, or future class to each time block based on the local time. 
  var currentTime = dayjs().hour();

  $(".description").each(function() {
    var taskTime = parseInt($(this).parent().attr("id").split("-")[1]);

    if (taskTime < currentTime){
      $(this).parent().addClass("past");
    } else if(taskTime == currentTime){
      $(this).parent().addClass("present");
    } else{
      $(this).parent().addClass("future");
    }  
  });

  // Gets the user input from localStorage and sets it to the corresponding time block.
  for (i = 9; i <= 17; i++) {
    $("#hour-" + i).children(".description").val(localStorage.getItem("hour-" + i));
  }

  // Code to display the current date in the header of the page.
  var currentDate = $("#currentDay");
      currentDate.text(dayjs().format("dddd, MMMM Do"));
});
