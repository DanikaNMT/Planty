package com.example.planty.ui

import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
import androidx.compose.runtime.Composable
import androidx.compose.ui.Modifier
import androidx.compose.ui.unit.dp
import com.example.planty.model.Plant

@Composable
fun plantOverview(plants: List<Plant>, onPlantClick: (Plant) -> Unit) {
    LazyColumn(modifier = Modifier.padding(16.dp)) {
        items(plants) { plant ->
            PlantCard (
                plant = plant,
                onClick = { onPlantClick(plant) }
            )
        }
    }
}