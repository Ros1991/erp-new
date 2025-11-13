import { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { MainLayout } from '../../components/layout';
import { Button } from '../../components/ui/Button';
import { Input } from '../../components/ui/Input';
import { Card, CardContent } from '../../components/ui/Card';
import { useToast } from '../../contexts/ToastContext';
import taskService from '../../services/taskService';
import { ArrowLeft, Save } from 'lucide-react';

interface TaskFormData {
  taskIdParent: string;
  taskIdBlocking: string;
  title: string;
  description: string;
  priority: string;
  frequencyDays: string;
  allowSunday: boolean;
  allowMonday: boolean;
  allowTuesday: boolean;
  allowWednesday: boolean;
  allowThursday: boolean;
  allowFriday: boolean;
  allowSaturday: boolean;
  startDate: string;
  endDate: string;
  overallStatus: string;
}

export function TaskForm() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { showError, showSuccess, handleBackendError } = useToast();
  const [isLoading, setIsLoading] = useState(false);
  const [isSaving, setIsSaving] = useState(false);
  
  const [formData, setFormData] = useState<TaskFormData>({
    taskIdParent: '',
    taskIdBlocking: '',
    title: '',
    description: '',
    priority: 'Média',
    frequencyDays: '',
    allowSunday: false,
    allowMonday: true,
    allowTuesday: true,
    allowWednesday: true,
    allowThursday: true,
    allowFriday: true,
    allowSaturday: false,
    startDate: '',
    endDate: '',
    overallStatus: 'Pendente',
  });

  const [errors, setErrors] = useState<Partial<Record<keyof TaskFormData, string>>>({});

  const isEditing = !!id;

  useEffect(() => {
    if (isEditing) {
      loadTask();
    }
  }, [id]);

  const loadTask = async () => {
    setIsLoading(true);
    try {
      const task = await taskService.getTaskById(Number(id));
      setFormData({
        taskIdParent: task.taskIdParent?.toString() || '',
        taskIdBlocking: task.taskIdBlocking?.toString() || '',
        title: task.title,
        description: task.description || '',
        priority: task.priority,
        frequencyDays: task.frequencyDays?.toString() || '',
        allowSunday: task.allowSunday,
        allowMonday: task.allowMonday,
        allowTuesday: task.allowTuesday,
        allowWednesday: task.allowWednesday,
        allowThursday: task.allowThursday,
        allowFriday: task.allowFriday,
        allowSaturday: task.allowSaturday,
        startDate: task.startDate ? task.startDate.split('T')[0] : '',
        endDate: task.endDate ? task.endDate.split('T')[0] : '',
        overallStatus: task.overallStatus,
      });
    } catch (err: any) {
      handleBackendError(err);
    } finally {
      setIsLoading(false);
    }
  };

  const validate = (): boolean => {
    const newErrors: Partial<Record<keyof TaskFormData, string>> = {};

    if (!formData.title.trim()) {
      newErrors.title = 'Título é obrigatório';
    }

    if (!formData.priority.trim()) {
      newErrors.priority = 'Prioridade é obrigatória';
    }

    if (!formData.overallStatus.trim()) {
      newErrors.overallStatus = 'Status geral é obrigatório';
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
      const taskData = {
        taskIdParent: formData.taskIdParent ? Number(formData.taskIdParent) : undefined,
        taskIdBlocking: formData.taskIdBlocking ? Number(formData.taskIdBlocking) : undefined,
        title: formData.title.trim(),
        description: formData.description.trim() || undefined,
        priority: formData.priority.trim(),
        frequencyDays: formData.frequencyDays ? Number(formData.frequencyDays) : undefined,
        allowSunday: formData.allowSunday,
        allowMonday: formData.allowMonday,
        allowTuesday: formData.allowTuesday,
        allowWednesday: formData.allowWednesday,
        allowThursday: formData.allowThursday,
        allowFriday: formData.allowFriday,
        allowSaturday: formData.allowSaturday,
        startDate: formData.startDate ? new Date(formData.startDate).toISOString() : undefined,
        endDate: formData.endDate ? new Date(formData.endDate).toISOString() : undefined,
        overallStatus: formData.overallStatus.trim(),
      };

      if (isEditing) {
        await taskService.updateTask(Number(id), taskData);
        showSuccess('Tarefa atualizada com sucesso!');
      } else {
        await taskService.createTask(taskData);
        showSuccess('Tarefa criada com sucesso!');
      }

      navigate('/tasks');
    } catch (err: any) {
      handleBackendError(err);
    } finally {
      setIsSaving(false);
    }
  };

  const handleChange = (field: keyof TaskFormData, value: string | boolean) => {
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
            onClick={() => navigate('/tasks')}
            className="mb-4"
          >
            <ArrowLeft className="h-4 w-4 mr-2" />
            Voltar
          </Button>
          <h1 className="text-3xl font-bold text-gray-900">
            {isEditing ? 'Editar Tarefa' : 'Nova Tarefa'}
          </h1>
          <p className="text-gray-600 mt-1">
            {isEditing ? 'Atualize as informações da tarefa' : 'Preencha as informações para criar uma nova tarefa'}
          </p>
        </div>

        <form onSubmit={handleSubmit} className="space-y-6">
          <Card>
            <CardContent className="p-6">
              <div className="space-y-4">
                <div>
                  <label htmlFor="title" className="block text-sm font-medium text-gray-700 mb-1">
                    Título <span className="text-red-500">*</span>
                  </label>
                  <Input
                    id="title"
                    type="text"
                    value={formData.title}
                    onChange={(e) => handleChange('title', e.target.value)}
                    placeholder="Ex: Revisar documentação"
                    className={errors.title ? 'border-red-500' : ''}
                  />
                  {errors.title && <p className="text-sm text-red-600 mt-1">{errors.title}</p>}
                </div>

                <div>
                  <label htmlFor="description" className="block text-sm font-medium text-gray-700 mb-1">
                    Descrição
                  </label>
                  <textarea
                    id="description"
                    value={formData.description}
                    onChange={(e) => handleChange('description', e.target.value)}
                    placeholder="Descrição detalhada da tarefa"
                    className="w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-2 focus:ring-primary-500 focus:border-primary-500"
                    rows={4}
                  />
                </div>

                <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                  <div>
                    <label htmlFor="priority" className="block text-sm font-medium text-gray-700 mb-1">
                      Prioridade <span className="text-red-500">*</span>
                    </label>
                    <Input
                      id="priority"
                      type="text"
                      value={formData.priority}
                      onChange={(e) => handleChange('priority', e.target.value)}
                      placeholder="Ex: Alta, Média, Baixa"
                      className={errors.priority ? 'border-red-500' : ''}
                    />
                    {errors.priority && <p className="text-sm text-red-600 mt-1">{errors.priority}</p>}
                  </div>

                  <div>
                    <label htmlFor="overallStatus" className="block text-sm font-medium text-gray-700 mb-1">
                      Status Geral <span className="text-red-500">*</span>
                    </label>
                    <Input
                      id="overallStatus"
                      type="text"
                      value={formData.overallStatus}
                      onChange={(e) => handleChange('overallStatus', e.target.value)}
                      placeholder="Ex: Pendente, Em Progresso, Concluída"
                      className={errors.overallStatus ? 'border-red-500' : ''}
                    />
                    {errors.overallStatus && <p className="text-sm text-red-600 mt-1">{errors.overallStatus}</p>}
                  </div>
                </div>

                <div className="grid grid-cols-1 sm:grid-cols-3 gap-4">
                  <div>
                    <label htmlFor="taskIdParent" className="block text-sm font-medium text-gray-700 mb-1">
                      ID Tarefa Pai (opcional)
                    </label>
                    <Input
                      id="taskIdParent"
                      type="text"
                      value={formData.taskIdParent}
                      onChange={(e) => handleChange('taskIdParent', e.target.value)}
                      placeholder="ID"
                    />
                  </div>

                  <div>
                    <label htmlFor="taskIdBlocking" className="block text-sm font-medium text-gray-700 mb-1">
                      ID Tarefa Bloqueante (opcional)
                    </label>
                    <Input
                      id="taskIdBlocking"
                      type="text"
                      value={formData.taskIdBlocking}
                      onChange={(e) => handleChange('taskIdBlocking', e.target.value)}
                      placeholder="ID"
                    />
                  </div>

                  <div>
                    <label htmlFor="frequencyDays" className="block text-sm font-medium text-gray-700 mb-1">
                      Frequência (dias) (opcional)
                    </label>
                    <Input
                      id="frequencyDays"
                      type="text"
                      value={formData.frequencyDays}
                      onChange={(e) => handleChange('frequencyDays', e.target.value)}
                      placeholder="Ex: 7"
                    />
                  </div>
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    Dias Permitidos
                  </label>
                  <div className="grid grid-cols-2 sm:grid-cols-4 gap-2">
                    <div className="flex items-center">
                      <input
                        id="allowMonday"
                        type="checkbox"
                        checked={formData.allowMonday}
                        onChange={(e) => handleChange('allowMonday', e.target.checked)}
                        className="h-4 w-4 text-primary-600 focus:ring-primary-500 border-gray-300 rounded"
                      />
                      <label htmlFor="allowMonday" className="ml-2 block text-sm text-gray-700">
                        Segunda
                      </label>
                    </div>
                    <div className="flex items-center">
                      <input
                        id="allowTuesday"
                        type="checkbox"
                        checked={formData.allowTuesday}
                        onChange={(e) => handleChange('allowTuesday', e.target.checked)}
                        className="h-4 w-4 text-primary-600 focus:ring-primary-500 border-gray-300 rounded"
                      />
                      <label htmlFor="allowTuesday" className="ml-2 block text-sm text-gray-700">
                        Terça
                      </label>
                    </div>
                    <div className="flex items-center">
                      <input
                        id="allowWednesday"
                        type="checkbox"
                        checked={formData.allowWednesday}
                        onChange={(e) => handleChange('allowWednesday', e.target.checked)}
                        className="h-4 w-4 text-primary-600 focus:ring-primary-500 border-gray-300 rounded"
                      />
                      <label htmlFor="allowWednesday" className="ml-2 block text-sm text-gray-700">
                        Quarta
                      </label>
                    </div>
                    <div className="flex items-center">
                      <input
                        id="allowThursday"
                        type="checkbox"
                        checked={formData.allowThursday}
                        onChange={(e) => handleChange('allowThursday', e.target.checked)}
                        className="h-4 w-4 text-primary-600 focus:ring-primary-500 border-gray-300 rounded"
                      />
                      <label htmlFor="allowThursday" className="ml-2 block text-sm text-gray-700">
                        Quinta
                      </label>
                    </div>
                    <div className="flex items-center">
                      <input
                        id="allowFriday"
                        type="checkbox"
                        checked={formData.allowFriday}
                        onChange={(e) => handleChange('allowFriday', e.target.checked)}
                        className="h-4 w-4 text-primary-600 focus:ring-primary-500 border-gray-300 rounded"
                      />
                      <label htmlFor="allowFriday" className="ml-2 block text-sm text-gray-700">
                        Sexta
                      </label>
                    </div>
                    <div className="flex items-center">
                      <input
                        id="allowSaturday"
                        type="checkbox"
                        checked={formData.allowSaturday}
                        onChange={(e) => handleChange('allowSaturday', e.target.checked)}
                        className="h-4 w-4 text-primary-600 focus:ring-primary-500 border-gray-300 rounded"
                      />
                      <label htmlFor="allowSaturday" className="ml-2 block text-sm text-gray-700">
                        Sábado
                      </label>
                    </div>
                    <div className="flex items-center">
                      <input
                        id="allowSunday"
                        type="checkbox"
                        checked={formData.allowSunday}
                        onChange={(e) => handleChange('allowSunday', e.target.checked)}
                        className="h-4 w-4 text-primary-600 focus:ring-primary-500 border-gray-300 rounded"
                      />
                      <label htmlFor="allowSunday" className="ml-2 block text-sm text-gray-700">
                        Domingo
                      </label>
                    </div>
                  </div>
                </div>

                <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                  <div>
                    <label htmlFor="startDate" className="block text-sm font-medium text-gray-700 mb-1">
                      Data de Início (opcional)
                    </label>
                    <Input
                      id="startDate"
                      type="date"
                      value={formData.startDate}
                      onChange={(e) => handleChange('startDate', e.target.value)}
                    />
                  </div>

                  <div>
                    <label htmlFor="endDate" className="block text-sm font-medium text-gray-700 mb-1">
                      Data de Término (opcional)
                    </label>
                    <Input
                      id="endDate"
                      type="date"
                      value={formData.endDate}
                      onChange={(e) => handleChange('endDate', e.target.value)}
                    />
                  </div>
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
                  {isEditing ? 'Atualizar' : 'Criar'} Tarefa
                </>
              )}
            </Button>
            <Button
              type="button"
              variant="outline"
              onClick={() => navigate('/tasks')}
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
