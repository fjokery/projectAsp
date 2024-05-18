function likePost(postindex) {
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
				let likenumberElement = document.getElementById("like-number-" + postindex);
				let likenumber = parseInt(likenumberElement.innerHTML);
				likenumberElement.innerHTML = likenumber + 1;
			} else if (xhr.status === 409) {
				// Request failed, handle errors if needed
				alert("You already liked this post.");
			} else {
				alert("There was an error liking this post.")
			}
		}
	};

	// Send the request
	xhr.send("postindex=" + postindex);
}