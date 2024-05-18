function likePost(forum, postindex) {
	// Create a new XMLHttpRequest object
	var xhr = new XMLHttpRequest();

	// Configure the request
	xhr.open("POST", "/Home/LikePost", true);
	xhr.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
	// Set the request headers if needed
	// xhr.setRequestHeader("Content-Type", "application/json");

	// Set up a callback function to handle the response
	xhr.onreadystatechange = function () {
		if (xhr.readyState === XMLHttpRequest.DONE) {
			if (xhr.status === 200) {

			} else {
				// Request failed, handle errors if needed
				alert("Failed to like post.")
			}
		}
	};

	// Send the request
	xhr.send("forum=" + forum + "&postindex=" + postindex);
}