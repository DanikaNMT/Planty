package com.example.planty.ui
import androidx.compose.foundation.layout.*
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.unit.dp
import androidx.lifecycle.viewmodel.compose.viewModel
import com.example.planty.model.Plant
import com.example.planty.viewmodel.PlantViewModel

@Composable
fun PlantScreen(
    viewModel: PlantViewModel = viewModel(),
    onPlantClick: (Plant) -> Unit
) {
    val plants by viewModel.plants
    val isLoading by viewModel.isLoading
    val error by viewModel.error

    Box(modifier = Modifier.fillMaxSize()) {
        when {
            isLoading -> {
                CircularProgressIndicator(
                    modifier = Modifier.align(Alignment.Center)
                )
            }

            error != null -> {
                Column(
                    modifier = Modifier.align(Alignment.Center),
                    horizontalAlignment = Alignment.CenterHorizontally
                ) {
                    Text("Error: $error")
                    Spacer(modifier = Modifier.height(16.dp))
                    Button(onClick = { viewModel.refreshPlants() }) {
                        Text("Retry")
                    }
                }
            }

            else -> {
                PlantOverview(
                    plants = plants,
                    onPlantClick = onPlantClick
                )
            }
        }
    }
}