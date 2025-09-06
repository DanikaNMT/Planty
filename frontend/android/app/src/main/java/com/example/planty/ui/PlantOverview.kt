package com.example.planty.ui

import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.Add
import androidx.compose.material3.*
import androidx.compose.runtime.Composable
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.unit.dp
import com.example.planty.model.Plant

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun PlantOverview(
    plants: List<Plant>,
    onPlantClick: (Plant) -> Unit,
    onAddPlantClick: (() -> Unit)? = null
) {
    Scaffold(
        containerColor = Color.White,
        topBar = {
            TopAppBar(
                title = {
                    Text(
                        text = "My Plants",
                        fontWeight = FontWeight.Bold
                    )
                },
                actions = {
                    onAddPlantClick?.let { addClick ->
                        IconButton(onClick = addClick) {
                            Icon(
                                Icons.Default.Add,
                                contentDescription = "Add Plant",
                                tint = Color.White
                            )
                        }
                    }
                },
                colors = TopAppBarDefaults.topAppBarColors(
                    containerColor = Color(0xFF80BF6F),
                    titleContentColor = Color.White
                )
            )
        }
    ) { paddingValues ->
        if (plants.isEmpty()) {
            // Empty state
            EmptyPlantsState(
                modifier = Modifier.padding(paddingValues),
                onAddPlantClick = onAddPlantClick
            )
        } else {
            LazyColumn(
                modifier = Modifier
                    .padding(paddingValues)
                    .padding(horizontal = 16.dp, vertical = 8.dp)
            ) {
                items(plants) { plant ->
                    PlantCard(
                        plant = plant,
                        onClick = { onPlantClick(plant) }
                    )
                }
            }
        }
    }
}

@Composable
private fun EmptyPlantsState(
    modifier: Modifier = Modifier,
    onAddPlantClick: (() -> Unit)?
) {
    Box(
        modifier = modifier
            .fillMaxSize()
            .padding(32.dp),
        contentAlignment = Alignment.Center
    ) {
        Column(
            horizontalAlignment = Alignment.CenterHorizontally
        ) {
            Text(
                text = "🌱",
                style = MaterialTheme.typography.displayLarge
            )
            Spacer(modifier = Modifier.height(16.dp))
            Text(
                text = "No plants yet",
                style = MaterialTheme.typography.headlineSmall,
                fontWeight = FontWeight.Bold
            )
            Spacer(modifier = Modifier.height(8.dp))
            Text(
                text = "Add your first plant to get started!",
                style = MaterialTheme.typography.bodyLarge,
                color = Color.Gray
            )

            onAddPlantClick?.let { addClick ->
                Spacer(modifier = Modifier.height(24.dp))
                Button(
                    onClick = addClick,
                    colors = ButtonDefaults.buttonColors(
                        containerColor = Color(0xFF80BF6F)
                    )
                ) {
                    Text("🌱 Add Your First Plant")
                }
            }
        }
    }
}