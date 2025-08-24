package com.example.planty.viewmodel

import androidx.compose.runtime.mutableStateOf
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.example.planty.model.Plant
import com.example.planty.repository.PlantRepository
import kotlinx.coroutines.launch
import androidx.compose.runtime.State

class PlantViewModel : ViewModel() {
    private val repository = PlantRepository()

    private val _plants = mutableStateOf<List<Plant>>(emptyList())
    val plants: State<List<Plant>> = _plants

    private val _isLoading = mutableStateOf(false)
    val isLoading: State<Boolean> = _isLoading

    private val _error = mutableStateOf<String?>(null)
    val error: State<String?> = _error

    // Add selected plant state
    private val _selectedPlant = mutableStateOf<Plant?>(null)
    val selectedPlant: State<Plant?> = _selectedPlant

    init {
        fetchAllPlants()
    }

    fun fetchAllPlants() {
        viewModelScope.launch {
            _isLoading.value = true
            _error.value = null

            val result = repository.getAllPlants()

            result.fold(
                onSuccess = { plantList ->
                    _plants.value = plantList
                    _isLoading.value = false
                },
                onFailure = { exception ->
                    _error.value = exception.message
                    _isLoading.value = false
                }
            )
        }
    }

    fun refreshPlants() {
        fetchAllPlants()
    }

    fun setSelectedPlant(plant: Plant) {
        _selectedPlant.value = plant
    }

    // Add plant creation method to this ViewModel too if you want to handle it here
    fun addPlant(name: String, plantSort: String, photo: String, onSuccess: () -> Unit = {}) {
        viewModelScope.launch {
            _isLoading.value = true
            _error.value = null

            val newPlant = Plant(
                id = 0, // Server will assign real ID
                name = name,
                photo = photo,
                plantSort = plantSort,
                lastWatered = null
            )

            val result = repository.createPlant(newPlant)

            result.fold(
                onSuccess = { createdPlant ->
                    // Refresh the plants list to include the new plant
                    fetchAllPlants()
                    onSuccess()
                },
                onFailure = { exception ->
                    _error.value = exception.message ?: "Failed to create plant"
                    _isLoading.value = false
                }
            )
        }
    }
}