package com.example.planty.ui

import androidx.compose.foundation.Image
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.runtime.Composable
import androidx.compose.ui.unit.dp
import com.example.planty.model.Plant
import androidx.compose.material3.Card
import androidx.compose.material3.CardDefaults
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.Text
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.layout.ContentScale
import androidx.compose.ui.text.font.FontWeight
import coil.compose.rememberAsyncImagePainter
import kotlinx.datetime.Clock
import kotlinx.datetime.DateTimeUnit
import kotlinx.datetime.TimeZone
import kotlinx.datetime.toLocalDateTime
import kotlinx.datetime.until

@Composable
fun PlantCard(plant: Plant, onClick: () -> Unit) {
    Card(
        modifier = Modifier
            .fillMaxWidth()
            .padding(vertical = 6.dp)
            .clickable { onClick() },
        elevation = CardDefaults.cardElevation(4.dp),
        colors = CardDefaults.cardColors(Color.White),
        shape = RoundedCornerShape(12.dp)
    ) {
        Row(
            modifier = Modifier
                .fillMaxWidth()
                .padding(16.dp),
            verticalAlignment = Alignment.CenterVertically
        ) {
            // Plant Image
            Card(
                modifier = Modifier.size(80.dp),
                elevation = CardDefaults.cardElevation(2.dp),
                shape = RoundedCornerShape(8.dp)
            ) {
                Image(
                    painter = rememberAsyncImagePainter(plant.photo),
                    contentDescription = plant.name,
                    modifier = Modifier
                        .fillMaxSize()
                        .clip(RoundedCornerShape(8.dp)),
                    contentScale = ContentScale.Crop
                )
            }

            Spacer(modifier = Modifier.width(16.dp))

            // Plant Information
            Column(
                modifier = Modifier.weight(1f)
            ) {
                // Plant Name
                Text(
                    text = plant.name,
                    style = MaterialTheme.typography.titleLarge,
                    fontWeight = FontWeight.Bold,
                    color = Color.Black
                )

                Spacer(modifier = Modifier.height(4.dp))

                // Plant Type
                Row(
                    verticalAlignment = Alignment.CenterVertically
                ) {
                    Text(
                        text = "🌱",
                        style = MaterialTheme.typography.bodyMedium
                    )
                    Spacer(modifier = Modifier.width(4.dp))
                    Text(
                        text = plant.plantSort,
                        style = MaterialTheme.typography.bodyMedium,
                        color = Color(0xFF666666)
                    )
                }

                Spacer(modifier = Modifier.height(4.dp))

                // Watering Status
                Row(
                    verticalAlignment = Alignment.CenterVertically
                ) {
                    Text(
                        text = "💧",
                        style = MaterialTheme.typography.bodyMedium
                    )
                    Spacer(modifier = Modifier.width(4.dp))
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
                                    val localDate = instant.toLocalDateTime(TimeZone.currentSystemDefault()).date
                                    "Last: ${localDate.month.name.take(3)} ${localDate.dayOfMonth}"
                                }
                            }
                        } ?: "Not watered yet",
                        style = MaterialTheme.typography.bodyMedium,
                        color = Color(0xFF666666)
                    )
                }
            }

            // Status Indicator (optional)
            Card(
                modifier = Modifier.size(12.dp),
                colors = CardDefaults.cardColors(
                    containerColor = plant.lastWatered?.let { instant ->
                        val now = Clock.System.now()
                        val duration = instant.until(now, DateTimeUnit.HOUR)
                        val daysDiff = (duration / 24).toInt()
                        when {
                            daysDiff <= 2 -> Color(0xFF4CAF50) // Green - recently watered
                            daysDiff <= 7 -> Color(0xFFFF9800) // Orange - needs attention
                            else -> Color(0xFFF44336) // Red - needs water
                        }
                    } ?: Color(0xFFE0E0E0), // Gray - never watered
                ),
                shape = RoundedCornerShape(6.dp),
                elevation = CardDefaults.cardElevation(0.dp)
            ) {}
        }
    }
}