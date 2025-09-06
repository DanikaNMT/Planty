package com.example.planty.viewmodel

import androidx.compose.runtime.State
import androidx.compose.runtime.mutableStateOf
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.example.planty.model.Plant
import com.example.planty.repository.PlantRepository
import kotlinx.coroutines.launch

class AddPlantViewModel : ViewModel() {
    private val repository = PlantRepository()

    private val _isLoading = mutableStateOf(false)
    val isLoading: State<Boolean> = _isLoading

    private val _error = mutableStateOf<String?>(null)
    val error: State<String?> = _error

    private val _plantCreated = mutableStateOf(false)
    val plantCreated: State<Boolean> = _plantCreated

    fun createPlant(name: String, plantSort: String, photo: String) {
        viewModelScope.launch {
            _isLoading.value = true
            _error.value = null

            // Create plant with temporary ID (server will assign real ID)
            val newPlant = Plant(
                id = 0, // Temporary ID, server will assign the real one
                name = name,
                photo = photo,
                plantSort = plantSort,
                lastWatered = null
            )

            val result = repository.createPlant(newPlant)

            result.fold(
                onSuccess = { createdPlant ->
                    _plantCreated.value = true
                    _isLoading.value = false
                },
                onFailure = { exception ->
                    _error.value = exception.message ?: "Failed to create plant"
                    _isLoading.value = false
                }
            )
        }
    }

    fun clearError() {
        _error.value = null
    }

    fun resetState() {
        _plantCreated.value = false
        _error.value = null
        _isLoading.value = false
    }
}