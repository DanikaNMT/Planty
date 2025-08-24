package com.example.planty

import android.os.Bundle
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import androidx.activity.enableEdgeToEdge
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.padding
import androidx.compose.material3.Scaffold
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.ui.Modifier
import androidx.compose.ui.tooling.preview.Preview
import com.example.planty.ui.PlantScreen
import com.example.planty.ui.theme.MyApplicationTheme
import com.example.planty.viewmodel.PlantViewModel

class MainActivity : ComponentActivity() {

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContent {
            PlantScreen(
                onPlantClick = { plant ->
                    // TODO Handle plant click - navigate to detail screen
                    println("Clicked on plant: ${plant.name}")
                }
            )
        }
    }
}