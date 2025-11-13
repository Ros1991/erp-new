import { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { MainLayout } from '../../components/layout';
import { Button } from '../../components/ui/Button';
import { Input } from '../../components/ui/Input';
import { Card, CardContent } from '../../components/ui/Card';
import { useToast } from '../../contexts/ToastContext';
import locationService from '../../services/locationService';
import { ArrowLeft, Save } from 'lucide-react';

interface LocationFormData {
  name: string;
  address: string;
  latitude: string;
  longitude: string;
  radiusMeters: string;
  isActive: boolean;
}

export function LocationForm() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { showError, showSuccess, handleBackendError } = useToast();
  const [isLoading, setIsLoading] = useState(false);
  const [isSaving, setIsSaving] = useState(false);
  
  const [formData, setFormData] = useState<LocationFormData>({
    name: '',
    address: '',
    latitude: '',
    longitude: '',
    radiusMeters: '100',
    isActive: true,
  });

  const [errors, setErrors] = useState<Partial<Record<keyof LocationFormData, string>>>({});

  const isEditing = !!id;

  useEffect(() => {
    if (isEditing) {
      loadLocation();
    }
  }, [id]);

  const loadLocation = async () => {
    setIsLoading(true);
    try {
      const location = await locationService.getLocationById(Number(id));
      setFormData({
        name: location.name,
        address: location.address || '',
        latitude: location.latitude.toString(),
        longitude: location.longitude.toString(),
        radiusMeters: location.radiusMeters.toString(),
        isActive: location.isActive,
      });
    } catch (err: any) {
      handleBackendError(err);
    } finally {
      setIsLoading(false);
    }
  };

  const validate = (): boolean => {
    const newErrors: Partial<Record<keyof LocationFormData, string>> = {};

    if (!formData.name.trim()) {
      newErrors.name = 'Nome é obrigatório';
    }

    if (!formData.latitude.trim()) {
      newErrors.latitude = 'Latitude é obrigatória';
    } else if (isNaN(Number(formData.latitude))) {
      newErrors.latitude = 'Latitude deve ser um número válido';
    }

    if (!formData.longitude.trim()) {
      newErrors.longitude = 'Longitude é obrigatória';
    } else if (isNaN(Number(formData.longitude))) {
      newErrors.longitude = 'Longitude deve ser um número válido';
    }

    if (!formData.radiusMeters.trim()) {
      newErrors.radiusMeters = 'Raio em metros é obrigatório';
    } else if (isNaN(Number(formData.radiusMeters)) || Number(formData.radiusMeters) <= 0) {
      newErrors.radiusMeters = 'Raio deve ser um número positivo';
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!validate()) {
      showError('Por favor, corrija os erros no formulário');
      return;
    }

    setIsSaving(true);
    try {
      const locationData = {
        name: formData.name.trim(),
        address: formData.address.trim() || undefined,
        latitude: Number(formData.latitude),
        longitude: Number(formData.longitude),
        radiusMeters: Number(formData.radiusMeters),
        isActive: formData.isActive,
      };

      if (isEditing) {
        await locationService.updateLocation(Number(id), locationData);
        showSuccess('Local atualizado com sucesso!');
      } else {
        await locationService.createLocation(locationData);
        showSuccess('Local criado com sucesso!');
      }

      navigate('/locations');
    } catch (err: any) {
      handleBackendError(err);
    } finally {
      setIsSaving(false);
    }
  };

  const handleChange = (field: keyof LocationFormData, value: string | boolean) => {
    setFormData(prev => ({ ...prev, [field]: value }));
    if (errors[field]) {
      setErrors(prev => ({ ...prev, [field]: undefined }));
    }
  };

  if (isLoading) {
    return (
      <MainLayout>
        <div className="flex items-center justify-center h-64">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary-600"></div>
        </div>
      </MainLayout>
    );
  }

  return (
    <MainLayout>
      <div className="max-w-2xl mx-auto">
        <div className="mb-6">
          <Button
            variant="ghost"
            onClick={() => navigate('/locations')}
            className="mb-4"
          >
            <ArrowLeft className="h-4 w-4 mr-2" />
            Voltar
          </Button>
          <h1 className="text-3xl font-bold text-gray-900">
            {isEditing ? 'Editar Local' : 'Novo Local'}
          </h1>
          <p className="text-gray-600 mt-1">
            {isEditing ? 'Atualize as informações do local' : 'Preencha as informações para criar um novo local'}
          </p>
        </div>

        <form onSubmit={handleSubmit} className="space-y-6">
          <Card>
            <CardContent className="p-6">
              <div className="space-y-4">
                <div>
                  <label htmlFor="name" className="block text-sm font-medium text-gray-700 mb-1">
                    Nome <span className="text-red-500">*</span>
                  </label>
                  <Input
                    id="name"
                    type="text"
                    value={formData.name}
                    onChange={(e) => handleChange('name', e.target.value)}
                    placeholder="Ex: Sede, Filial Centro, Depósito"
                    className={errors.name ? 'border-red-500' : ''}
                  />
                  {errors.name && <p className="text-sm text-red-600 mt-1">{errors.name}</p>}
                </div>

                <div>
                  <label htmlFor="address" className="block text-sm font-medium text-gray-700 mb-1">
                    Endereço
                  </label>
                  <textarea
                    id="address"
                    value={formData.address}
                    onChange={(e) => handleChange('address', e.target.value)}
                    placeholder="Endereço completo do local"
                    className="w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-2 focus:ring-primary-500 focus:border-primary-500"
                    rows={2}
                  />
                </div>

                <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                  <div>
                    <label htmlFor="latitude" className="block text-sm font-medium text-gray-700 mb-1">
                      Latitude <span className="text-red-500">*</span>
                    </label>
                    <Input
                      id="latitude"
                      type="text"
                      value={formData.latitude}
                      onChange={(e) => handleChange('latitude', e.target.value)}
                      placeholder="-23.550520"
                      className={errors.latitude ? 'border-red-500' : ''}
                    />
                    {errors.latitude && <p className="text-sm text-red-600 mt-1">{errors.latitude}</p>}
                  </div>

                  <div>
                    <label htmlFor="longitude" className="block text-sm font-medium text-gray-700 mb-1">
                      Longitude <span className="text-red-500">*</span>
                    </label>
                    <Input
                      id="longitude"
                      type="text"
                      value={formData.longitude}
                      onChange={(e) => handleChange('longitude', e.target.value)}
                      placeholder="-46.633308"
                      className={errors.longitude ? 'border-red-500' : ''}
                    />
                    {errors.longitude && <p className="text-sm text-red-600 mt-1">{errors.longitude}</p>}
                  </div>
                </div>

                <div>
                  <label htmlFor="radiusMeters" className="block text-sm font-medium text-gray-700 mb-1">
                    Raio em Metros <span className="text-red-500">*</span>
                  </label>
                  <Input
                    id="radiusMeters"
                    type="text"
                    value={formData.radiusMeters}
                    onChange={(e) => handleChange('radiusMeters', e.target.value)}
                    placeholder="100"
                    className={errors.radiusMeters ? 'border-red-500' : ''}
                  />
                  {errors.radiusMeters && <p className="text-sm text-red-600 mt-1">{errors.radiusMeters}</p>}
                  <p className="text-sm text-gray-500 mt-1">Raio de tolerância para registro de ponto</p>
                </div>

                <div className="flex items-center">
                  <input
                    id="isActive"
                    type="checkbox"
                    checked={formData.isActive}
                    onChange={(e) => handleChange('isActive', e.target.checked)}
                    className="h-4 w-4 text-primary-600 focus:ring-primary-500 border-gray-300 rounded"
                  />
                  <label htmlFor="isActive" className="ml-2 block text-sm text-gray-700">
                    Local ativo
                  </label>
                </div>
              </div>

            </CardContent>
          </Card>

          <div className="flex gap-3 justify-end">
                <Button type="submit" disabled={isSaving} className="flex-1 sm:flex-none">
                  {isSaving ? (
                    <>
                      <div className="animate-spin rounded-full h-4 w-4 border-b-2 border-white mr-2"></div>
                      Salvando...
                    </>
                  ) : (
                    <>
                      <Save className="h-4 w-4 mr-2" />
                      {isEditing ? 'Atualizar' : 'Criar'} Local
                    </>
                  )}
                </Button>
                <Button
                  type="button"
                  variant="outline"
                  onClick={() => navigate('/locations')}
                  disabled={isSaving}
                  className="flex-1 sm:flex-none"
                >
                  Cancelar
                </Button>
          </div>
        </form>
      </div>
    </MainLayout>
  );
}
