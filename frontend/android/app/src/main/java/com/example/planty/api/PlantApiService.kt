package com.example.planty.api

import com.example.planty.model.Plant
import retrofit2.Response
import retrofit2.http.*

interface PlantApiService {
    @GET("plants")
    suspend fun getAllPlants(): Response<List<Plant>>

    @GET("plants/{id}")
    suspend fun getPlant(@Path("id") id: Int): Response<Plant>

    @POST("plants")
    suspend fun createPlant(@Body plant: Plant): Response<Plant>

    @PUT("plants/{id}")
    suspend fun updatePlant(@Path("id") id: Int, @Body plant: Plant): Response<Plant>

    @DELETE("plants/{id}")
    suspend fun deletePlant(@Path("id") id: Int): Response<Unit>
}