package com.example.planty.ui

import androidx.compose.foundation.Image
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.rememberScrollState
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.foundation.verticalScroll
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.ArrowBack
import androidx.compose.material.icons.filled.Home
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.layout.ContentScale
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.unit.dp
import coil.compose.rememberAsyncImagePainter
import com.example.planty.viewmodel.PlantViewModel
import kotlinx.datetime.Clock
import kotlinx.datetime.DateTimeUnit
import kotlinx.datetime.TimeZone
import kotlinx.datetime.toLocalDateTime
import kotlinx.datetime.until

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun PlantDetailScreenSimple(
    plantViewModel: PlantViewModel,
    onBackClick: () -> Unit
) {
    val selectedPlant by plantViewModel.selectedPlant

    Scaffold(
        containerColor = Color(0xFFF8F9FA),
        topBar = {
            TopAppBar(
                title = {
                    Text(
                        text = selectedPlant?.name ?: "Plant Details",
                        fontWeight = FontWeight.Bold
                    )
                },
                navigationIcon = {
                    IconButton(onClick = onBackClick) {
                        Icon(
                            Icons.Default.ArrowBack,
                            contentDescription = "Back",
                            tint = Color.White
                        )
                    }
                },
                colors = TopAppBarDefaults.topAppBarColors(
                    containerColor = Color(0xFF80BF6F),
                    titleContentColor = Color.White
                )
            )
        }
    ) { paddingValues ->
        selectedPlant?.let { plant ->
            Column(
                modifier = Modifier
                    .fillMaxSize()
                    .padding(paddingValues)
                    .verticalScroll(rememberScrollState())
                    .padding(16.dp)
            ) {
                // Plant Image
                Card(
                    modifier = Modifier
                        .fillMaxWidth()
                        .height(250.dp),
                    elevation = CardDefaults.cardElevation(8.dp)
                ) {
                    Image(
                        painter = rememberAsyncImagePainter(plant.photo),
                        contentDescription = plant.name,
                        modifier = Modifier
                            .fillMaxSize()
                            .clip(RoundedCornerShape(12.dp)),
                        contentScale = ContentScale.Crop
                    )
                }

                Spacer(modifier = Modifier.height(24.dp))

                // Plant Name
                Text(
                    text = plant.name,
                    style = MaterialTheme.typography.headlineMedium,
                    fontWeight = FontWeight.Bold,
                    modifier = Modifier.padding(bottom = 8.dp)
                )

                // Plant Type Card
                Card(
                    modifier = Modifier.fillMaxWidth(),
                    colors = CardDefaults.cardColors(Color(0xFFE8F5E8)),
                    elevation = CardDefaults.cardElevation(4.dp)
                ) {
                    Column(
                        modifier = Modifier.padding(16.dp)
                    ) {
                        Row(
                            verticalAlignment = Alignment.CenterVertically,
                            modifier = Modifier.padding(bottom = 8.dp)
                        ) {
                            Text(
                                text = "🌱",
                                style = MaterialTheme.typography.titleLarge
                            )
                            Spacer(modifier = Modifier.width(8.dp))
                            Text(
                                text = "Plant Type",
                                style = MaterialTheme.typography.titleMedium,
                                fontWeight = FontWeight.SemiBold
                            )
                        }
                        Text(
                            text = plant.plantSort,
                            style = MaterialTheme.typography.bodyLarge
                        )
                    }
                }

                Spacer(modifier = Modifier.height(16.dp))

                // Last Watered Card
                Card(
                    modifier = Modifier.fillMaxWidth(),
                    colors = CardDefaults.cardColors(Color(0xFFF0F8FF)),
                    elevation = CardDefaults.cardElevation(4.dp)
                ) {
                    Column(
                        modifier = Modifier.padding(16.dp)
                    ) {
                        Row(
                            verticalAlignment = Alignment.CenterVertically,
                            modifier = Modifier.padding(bottom = 8.dp)
                        ) {
                            Text(
                                text = "💧",
                                style = MaterialTheme.typography.titleLarge
                            )
                            Spacer(modifier = Modifier.width(8.dp))
                            Text(
                                text = "Watering Info",
                                style = MaterialTheme.typography.titleMedium,
                                fontWeight = FontWeight.SemiBold
                            )
                        }
                        Text(
                            text = plant.lastWatered?.let { instant ->
                                val now = Clock.System.now()
                                val duration = instant.until(now, DateTimeUnit.HOUR)
                                val daysDiff = (duration / 24).toInt()
                                when {
                                    daysDiff == 0 -> "Watered today"
                                    daysDiff == 1 -> "Watered yesterday"
                                    daysDiff < 7 -> "Watered $daysDiff days ago"
                                    else -> {
                                        val localDate =
                                            instant.toLocalDateTime(TimeZone.currentSystemDefault()).date
                                        "Last watered: ${localDate.month.name.take(3)} ${localDate.dayOfMonth}"
                                    }
                                }
                            } ?: "Not watered yet",
                            style = MaterialTheme.typography.bodyLarge
                        )

                        // Water button
                        Spacer(modifier = Modifier.height(12.dp))
                        Button(
                            onClick = { /* TODO: Implement water plant functionality */ },
                            modifier = Modifier.fillMaxWidth(),
                            colors = ButtonDefaults.buttonColors(
                                containerColor = Color(0xFF4CAF50)
                            )
                        ) {
                            Text("💧 Water Plant")
                        }
                    }
                }

                Spacer(modifier = Modifier.height(16.dp))
            }
        } ?: run {
            // Show error if no plant selected
            Box(
                modifier = Modifier
                    .fillMaxSize()
                    .padding(paddingValues),
                contentAlignment = Alignment.Center
            ) {
                Column(
                    horizontalAlignment = Alignment.CenterHorizontally
                ) {
                    Text("No plant selected")
                    Spacer(modifier = Modifier.height(16.dp))
                    Button(onClick = onBackClick) {
                        Text("Go Back")
                    }
                }
            }
        }
    }
}