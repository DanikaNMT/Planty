package com.example.planty.model

import kotlinx.datetime.Instant
import kotlinx.serialization.Serializable

@Serializable
data class Plant(
    val id: Int,
    val name: String,
    val photo: String,
    val plantSort: String,
    val lastWatered: Instant? = null
)