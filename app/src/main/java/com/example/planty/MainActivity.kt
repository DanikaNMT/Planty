package com.example.planty

import android.os.Bundle
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import androidx.compose.runtime.Composable
import androidx.navigation.compose.NavHost
import androidx.navigation.compose.composable
import androidx.navigation.compose.rememberNavController
import com.example.planty.ui.PlantDetailScreenSimple
import com.example.planty.ui.PlantScreen
import com.example.planty.ui.theme.MyApplicationTheme
import androidx.lifecycle.viewmodel.compose.viewModel
import com.example.planty.viewmodel.PlantViewModel

class MainActivity : ComponentActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContent {
            MyApplicationTheme {
                PlantAppSimple()
            }
        }
    }
}

@Composable
fun PlantAppSimple() {
    val navController = rememberNavController()
    val plantViewModel: PlantViewModel = viewModel()

    NavHost(
        navController = navController,
        startDestination = "plant_list"
    ) {
        composable("plant_list") {
            PlantScreen(
                viewModel = plantViewModel,
                onPlantClick = { plant ->
                    // Store selected plant in ViewModel
                    plantViewModel.setSelectedPlant(plant)
                    navController.navigate("plant_detail")
                }
            )
        }

        composable("plant_detail") {
            PlantDetailScreenSimple(
                plantViewModel = plantViewModel,
                onHomeClick = {
                    navController.navigate("plant_list") {
                        popUpTo("plant_list") { inclusive = true }
                    }
                }
            )
        }
    }
}