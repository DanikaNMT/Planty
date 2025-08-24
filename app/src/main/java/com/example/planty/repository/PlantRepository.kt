package com.example.planty.repository
import com.example.planty.api.ApiClient
import com.example.planty.api.PlantApiService
import com.example.planty.model.Plant
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.withContext

class PlantRepository(private val apiService: PlantApiService = ApiClient.plantApiService) {

    suspend fun getAllPlants(): Result<List<Plant>> {
        return withContext(Dispatchers.IO) {
            try {
                val response = apiService.getAllPlants()
                if (response.isSuccessful && response.body() != null) {
                    Result.success(response.body()!!)
                } else {
                    Result.failure(Exception("Failed to fetch plants: ${response.message()}"))
                }
            } catch (e: Exception) {
                Result.failure(e)
            }
        }
    }

    suspend fun getPlant(id: Int): Result<Plant> {
        return withContext(Dispatchers.IO) {
            try {
                val response = apiService.getPlant(id)
                if (response.isSuccessful && response.body() != null) {
                    Result.success(response.body()!!)
                } else {
                    Result.failure(Exception("Failed to fetch plant: ${response.message()}"))
                }
            } catch (e: Exception) {
                Result.failure(e)
            }
        }
    }

    suspend fun createPlant(plant: Plant): Result<Plant> {
        return withContext(Dispatchers.IO) {
            try {
                val response = apiService.createPlant(plant)
                if (response.isSuccessful && response.body() != null) {
                    Result.success(response.body()!!)
                } else {
                    Result.failure(Exception("Failed to create plant: ${response.message()}"))
                }
            } catch (e: Exception) {
                Result.failure(e)
            }
        }
    }

    suspend fun updatePlant(id: Int, plant: Plant): Result<Plant> {
        return withContext(Dispatchers.IO) {
            try {
                val response = apiService.updatePlant(id, plant)
                if (response.isSuccessful && response.body() != null) {
                    Result.success(response.body()!!)
                } else {
                    Result.failure(Exception("Failed to update plant: ${response.message()}"))
                }
            } catch (e: Exception) {
                Result.failure(e)
            }
        }
    }

    suspend fun deletePlant(id: Int): Result<Unit> {
        return withContext(Dispatchers.IO) {
            try {
                val response = apiService.deletePlant(id)
                if (response.isSuccessful) {
                    Result.success(Unit)
                } else {
                    Result.failure(Exception("Failed to delete plant: ${response.message()}"))
                }
            } catch (e: Exception) {
                Result.failure(e)
            }
        }
    }
}