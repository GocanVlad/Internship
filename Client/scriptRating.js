"use strict";
const baseUrl = "http://localhost:1028/api/movies/";
let currentId, entityIndex;

const myForm = document.getElementById("myForm");
const modal = document.querySelector(".modal");
const overlay = document.querySelector(".overlay");
const postM = document.querySelectorAll(".show");
const editBtn = document.getElementById("edit");

editBtn.addEventListener("click", updateRating);
overlay.addEventListener("click", closeModal);
getAllMovies();

myForm.addEventListener("submit",async function postRating(e) {
  e.preventDefault();

  let nameInputValue = document.getElementById("name").value;
  let descriptionInputValue = document.getElementById("description").value;
  const movieListOptions = document.getElementById("movies-list").options;
  const movieId = movieListOptions[movieListOptions.selectedIndex].id;

  if(movieId !== 'empty')
  {
    await fetch(baseUrl + `${movieId}/ratings/`, {
      method: "post",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        name: nameInputValue,
        description: descriptionInputValue,
      }),
    });
    closeModal();
    await getAllRatings(movieId);
    document.myForm.reset();
  }
});

async function getAllMovies() {
  document.getElementById('create-rating').style.opacity="0"
  await fetch(baseUrl)
    .then((res) => res.json())
    .then((data) => {
      renderMovie(data);
    })
    .catch((error) => {
      console.log(error);
    });
}

async function getAllRatings(movieId) {
  await fetch(baseUrl + `${movieId}/ratings/`)
    .then((res) => res.json())
    .then((data) => {
      renderRating(data);
    })
    .catch((error) => {
      console.log(error);
    });
  }

const renderMovie = function (data) {
  let listBodyContent = `
    <label for="ratings">Choose a movie:</label>
      <select id="movies-list" title="movies-list" name="movies-list" onchange="selectedItem();" >
        <option id="empty" value="empty"></option>
    `;
  for (let i = 0; i < data.length; i++) {
    listBodyContent += ` 
      <option id="${data[i].id}" value="${data[i].name}">${data[i].name}</option>`;
  }
  document.getElementById("list-body").innerHTML = listBodyContent;
};

const renderRating = function (data) {
  const  movieListOptions= document.getElementById("movies-list").options;
  let tableBodyContent = "";
  for (let i = 0; i < data.length; i++) {
    tableBodyContent += `
      <tr>
      <td id="rating-id" style="display:none">${data[i].id}</td>
        <td id="rating-name ${data[i].id}">${data[i].name}</td>
        <td id="rating-description ${data[i].id}">${data[i].description}</td>
        <td><button onclick="deleteRatingById(${movieListOptions[movieListOptions.selectedIndex].id},${data[i].id})" id="delete-rating" style="background-color:rgb(206, 87, 50)">🗑️</button>
        <button onclick="updateCurrentId(${data[i].id});
        openUpdateModal();
        showValues();" class="modal-winodw" id="update-rating" style="background-color:#1e81d3">
          Update Rating
        </button></td>
      </tr>
  `;
  }
  document.getElementById("rating-table").innerHTML = tableBodyContent;
};

async function deleteRatingById(movieId, id) {
  await fetch(baseUrl + `${movieId}/ratings/${id}`, { method: "delete" }).catch(
    (error) => {
      throw new Error("NaN");
    }
  );
  getAllRatings(movieId);
}

async function updateRating() {
  const data = getFieldValue();
  const putMethod = {
    method: "PUT",
    headers: {
      "Content-type": "application/json; charset=UTF-8",
    },
    body: JSON.stringify(getFieldValue()),
  };

  const movieListOptions = document.getElementById("movies-list").options;
  await fetch(baseUrl + `${movieListOptions[movieListOptions.selectedIndex].id}/ratings/${currentId}`, putMethod)
    .then((response) => {
      return response.text();
    })
    .catch((error) => {
      throw new Error("bad");
    });

  closeModal();
  await getAllRatings(movieListOptions[movieListOptions.selectedIndex].id)
  document.myForm.reset();
  
}

function selectedItem() {
  const movieListOptions = document.getElementById("movies-list").options;
  entityIndex = movieListOptions[movieListOptions.selectedIndex].id;
  try {
    if (entityIndex !== "empty") {
      document.getElementById('create-rating').style.opacity="1"
      updateCurrentId(entityIndex);
      getAllRatings(entityIndex);
    } else {
       document.getElementById("rating-table").innerHTML = "<tr></tr>";
       document.getElementById('create-rating').style.opacity="0"
    }
  } catch (error) {
    console.log("Empty Option");
  }
}

function updateCurrentId(id) {
  currentId = id;
}

function showValues() {
  const data = initializeField();
  document.getElementById("name").value = data.name;
  document.getElementById("description").value = data.description;
}

function initializeField() {
  const data = {
    name: document.getElementById(`rating-name ${currentId}`).textContent,
    description: document.getElementById(`rating-description ${currentId}`).textContent
  };
  return data;
}

function getFieldValue() {
  const data = {
    name: document.getElementById(`name`).value,
    description: document.getElementById(`description`).value
  };
  return data;
}

function openCreateModal() {
  modal.classList.remove("hidden");
  overlay.classList.remove("hidden");
  document.getElementById("edit").style.display = "none";
};

function openUpdateModal() {
  modal.classList.remove("hidden");
  overlay.classList.remove("hidden");
  document.getElementById("submit").style.display = "none";
  document.getElementById("submit").disabled = true;
};

function closeModal() {
  resetBtn();
  modal.classList.add("hidden");
  overlay.classList.add("hidden");
  
};

const resetBtn =function() {
  document.getElementById("edit").style.display = "initial";
  document.getElementById("submit").style.display = "initial";
  document.getElementById("submit").disabled = false;
}

