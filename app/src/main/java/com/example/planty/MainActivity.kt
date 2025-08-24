package com.example.planty

import android.os.Bundle
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import androidx.compose.runtime.*
import androidx.navigation.compose.NavHost
import androidx.navigation.compose.composable
import androidx.navigation.compose.rememberNavController
import com.example.planty.ui.AddPlantScreen
import com.example.planty.ui.PlantDetailScreenSimple
import com.example.planty.ui.PlantScreen
import com.example.planty.ui.theme.MyApplicationTheme
import androidx.lifecycle.viewmodel.compose.viewModel
import com.example.planty.viewmodel.AddPlantViewModel
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
                },
                onAddPlantClick = {  // Add this parameter!
                    navController.navigate("add_plant")
                }
            )
        }

        composable("plant_detail") {
            PlantDetailScreenSimple(
                plantViewModel = plantViewModel,
                onBackClick = {
                    navController.popBackStack()
                },
            )
        }

        composable("add_plant") {
            val addPlantViewModel: AddPlantViewModel = viewModel()
            val isLoading by addPlantViewModel.isLoading
            val error by addPlantViewModel.error
            val plantCreated by addPlantViewModel.plantCreated

            LaunchedEffect(plantCreated) {
                if (plantCreated) {
                    plantViewModel.refreshPlants()
                    addPlantViewModel.resetState()
                    navController.popBackStack()
                }
            }

            AddPlantScreen(
                onBackClick = {
                    navController.popBackStack()
                },
                onSavePlant = { name, plantSort, photo ->
                    addPlantViewModel.createPlant(name, plantSort, photo)
                },
                isLoading = isLoading,
                error = error
            )
        }
    }
}