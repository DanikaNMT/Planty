package com.example.planty.viewmodel

import androidx.compose.runtime.mutableStateOf
import androidx.lifecycle.LiveData
import androidx.lifecycle.MutableLiveData
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

    init {
        fetchAllPlants()
    }

    private fun fetchAllPlants() {
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
}