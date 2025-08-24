package com.example.planty.ui

import androidx.compose.foundation.Image
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.size
import androidx.compose.runtime.Composable
import androidx.compose.ui.unit.dp
import com.example.planty.model.Plant
import androidx.compose.material3.Card
import androidx.compose.material3.CardDefaults
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.Text
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.res.stringResource
import coil.compose.rememberAsyncImagePainter


@Composable
fun PlantCard(plant: Plant, onClick: () -> Unit) {
    Card(
        modifier = Modifier
            .fillMaxWidth()
            .padding(5.dp)
            .clickable { onClick() },
        elevation = CardDefaults.cardElevation(0.dp),
        colors = CardDefaults.cardColors(Color(0xA0f2dac4))
    ) {
        Row(
            modifier = Modifier.padding(16.dp),
            verticalAlignment = Alignment.CenterVertically
        ) {
            // Image on the left
            Image(
                painter = rememberAsyncImagePainter(plant.photo),
                contentDescription = null,
                modifier = Modifier
                    .size(70.dp)
                    .padding(end = 10.dp)
            )

            // Text on the right
            Column {
                Text(
                    text = plant.name,
                    style = MaterialTheme.typography.titleLarge
                )

                Text(
                    text = "🌱 " + plant.plantSort,
                    modifier = Modifier.padding(10.dp, 1.dp, 0.dp, 1.dp),
                    style = MaterialTheme.typography.bodyLarge
                )
            }
        }
    }
}