// PlantDetailViewModel.kt - Updated to get plant from the plants list
package com.example.planty.viewmodel

import androidx.compose.runtime.mutableStateOf
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.example.planty.model.Plant
import com.example.planty.repository.PlantRepository
import kotlinx.coroutines.launch
import androidx.compose.runtime.State

class PlantDetailViewModel : ViewModel() {
    private val repository = PlantRepository()

    private val _plant = mutableStateOf<Plant?>(null)
    val plant: State<Plant?> = _plant

    private val _isLoading = mutableStateOf(false)
    val isLoading: State<Boolean> = _isLoading

    private val _error = mutableStateOf<String?>(null)
    val error: State<String?> = _error

    fun fetchPlant(id: Int) {
        viewModelScope.launch {
            _isLoading.value = true
            _error.value = null

            // First try to get individual plant
            val individualResult = repository.getPlant(id)

            if (individualResult.isSuccess) {
                _plant.value = individualResult.getOrNull()
                _isLoading.value = false
            } else {
                // If individual plant fetch fails, get from plants list
                val allPlantsResult = repository.getAllPlants()

                allPlantsResult.fold(
                    onSuccess = { plantList ->
                        val foundPlant = plantList.find { it.id == id }
                        if (foundPlant != null) {
                            _plant.value = foundPlant
                            _isLoading.value = false
                        } else {
                            _error.value = "Plant with ID $id not found"
                            _isLoading.value = false
                        }
                    },
                    onFailure = { exception ->
                        _error.value = exception.message
                        _isLoading.value = false
                    }
                )
            }
        }
    }
}